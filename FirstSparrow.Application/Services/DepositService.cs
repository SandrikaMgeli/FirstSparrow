using System.Data;
using System.Numerics;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Domain.Models;
using FirstSparrow.Application.Extensions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Application.Services.Abstractions;

namespace FirstSparrow.Application.Services;

public class DepositService(
    IMerkleNodeRepository merkleNodeRepository,
    TimeProvider timeProvider,
    IBlockChainService blockChainService,
    IDbManager dbManager) : IDepositService
{
    public async Task ProcessDeposit(Deposit deposit, CancellationToken cancellationToken)
    {
        await using IDbManagementContext context = await dbManager.RunWithTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        
        deposit.EnsureNodeIsDeposit();
        await EnsureDepositNotExists(deposit, cancellationToken);

        await EnsurePreviousDepositExists(deposit, cancellationToken);

        await merkleNodeRepository.Insert(deposit, cancellationToken);

        await UpdateTreeRecursively(deposit, cancellationToken);
    }

    private async Task EnsureDepositNotExists(Deposit deposit, CancellationToken cancellationToken)
    {
        MerkleNode? currentNode = await merkleNodeRepository.GetByNodeCoordinate(deposit.Index, deposit.Layer, false, cancellationToken);
        if (currentNode is not null)
        {
            throw new AppException("Deposit already exists", ExceptionCode.DEPOSIT_ALREADY_EXISTS);
        }
    }

    private async Task EnsurePreviousDepositExists(MerkleNode merkleNode, CancellationToken cancellationToken)
    {
        if (merkleNode.Index == 0)
        {
            return;
        }

        long previousDepositIndex = merkleNode.CalculatePreviousDepositIndex();

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
        (long parentIndex, int parentLayer) = merkleNode.CalculateParentCoordinate();

        MerkleNode? parentNode = await merkleNodeRepository.GetByNodeCoordinate(parentIndex, parentLayer, false, cancellationToken);

        BigInteger parentNewCommitment = await blockChainService.HashConcat(merkleNode.GetCommitmentAsBigInteger(), neighbour.GetCommitmentAsBigInteger());

        if (parentNode is null)
        {
            // Create parent
            parentNode = new MerkleNode(parentLayer)
            {
                Commitment = parentNewCommitment.ToHex(),
                Index = parentIndex,
                CreateTimestamp = timeProvider.GetUtcNowDateTime(),
                UpdateTimestamp = null,
                IsDeleted = false,
                DepositTimestamp = null,
            };
            await merkleNodeRepository.Insert(parentNode, cancellationToken);
            return parentNode;
        }

        // Update parent
        parentNode.Commitment = parentNewCommitment.ToHex();
        parentNode.UpdateTimestamp = timeProvider.GetUtcNowDateTime();
        await merkleNodeRepository.Update(parentNode, true, cancellationToken);

        return parentNode;
    }
}