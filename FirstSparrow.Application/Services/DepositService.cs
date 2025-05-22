using System.Numerics;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Domain.Models;
using FirstSparrow.Application.Extensions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Services.Abstractions;

namespace FirstSparrow.Application.Services;

public class DepositService(
    IMerkleNodeRepository merkleNodeRepository,
    TimeProvider timeProvider,
    IBlockChainService blockChainService) : IDepositService
{
    public async Task ProcessDeposit(Deposit deposit, CancellationToken cancellationToken)
    {
        EnsureNodeIsDeposit(deposit);
        await EnsurePreviousDepositExists(deposit, cancellationToken);

        await merkleNodeRepository.Insert(deposit, cancellationToken);

        await UpdateTreeRecursively(deposit, cancellationToken);
    }

    private static void EnsureNodeIsDeposit(MerkleNode merkleNode)
    {
        if (merkleNode.Layer != 0)
        {
            throw new AppException("Merkle node layer must be zero", ExceptionCode.GENERAL_ERROR);
        }
    }

    private async Task EnsurePreviousDepositExists(MerkleNode merkleNode, CancellationToken cancellationToken)
    {
        if (merkleNode.Index == 0)
        {
            return;
        }

        uint previousDepositIndex = merkleNode.CalculatePreviousDepositIndex();

        MerkleNode? previousNode = await merkleNodeRepository.GetByNodeCoordinate(index: previousDepositIndex, layer: 0, ensureExists: false, cancellationToken);

        if (previousNode is null)
        {
            throw new AppException($"previous index doesn't exists with layer : 0 and index : {previousDepositIndex}", ExceptionCode.GENERAL_ERROR);
        }
    }

    private async Task UpdateTreeRecursively(MerkleNode merkleNode, CancellationToken cancellationToken)
    {
        if (merkleNode.IsRoot) return;

        MerkleNode neighbour = await merkleNodeRepository.GetNeighbour(merkleNode, cancellationToken);

        MerkleNode parentNode = await UpdateOrCreateParent(merkleNode, neighbour, cancellationToken);

        await UpdateTreeRecursively(parentNode, cancellationToken);
    }

    private async Task<MerkleNode> UpdateOrCreateParent(MerkleNode merkleNode, MerkleNode neighbour, CancellationToken cancellationToken)
    {
        (uint parentIndex, int parentLayer) = merkleNode.CalculateParentCoordinate();

        MerkleNode? parentNode = await merkleNodeRepository.GetByNodeCoordinate(parentIndex, parentLayer, false, cancellationToken);

        BigInteger parentNewCommitment = await blockChainService.HashConcat(merkleNode.GetCommitmentAsBigInteger(), neighbour.GetCommitmentAsBigInteger());

        if (parentNode is null)
        {
            // Create parent
            parentNode = new MerkleNode(parentLayer)
            {
                Commitment = parentNewCommitment.ToHex(),
                Index = parentIndex,
                CreateTimestamp = timeProvider.GetUtcNow().DateTime,
                UpdateTimestamp = null,
                IsDeleted = false,
                DepositTimestamp = null,
            };
            await merkleNodeRepository.Insert(parentNode, cancellationToken);
            return parentNode;
        }

        // Update parent
        parentNode.Commitment = parentNewCommitment.ToHex();
        parentNode.UpdateTimestamp = timeProvider.GetUtcNow().DateTime;
        await merkleNodeRepository.Update(parentNode, true, cancellationToken);

        return parentNode;
    }
}