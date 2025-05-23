using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Extensions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using MediatR;

namespace FirstSparrow.Application.Features.Deposits.GetDepositDetails;

public class GetDepositDetailsQueryHandler(
    IMerkleNodeRepository merkleNodeRepository,
    IDbManager dbManager) : IRequestHandler<GetDepositDetailsQuery, GetDepositDetailsResponse>
{
    public async Task<GetDepositDetailsResponse> Handle(GetDepositDetailsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Take a lock here.
        await using IDbManagementContext context = await dbManager.RunAsync(cancellationToken);

        MerkleNode deposit = await EnsureAndGetDeposit(request, cancellationToken);

        List<MerkleNode> path = [];
        List<int> indices = [];

        string root = await PopulatePathAndIndicesRecursively(deposit, path, indices, cancellationToken);

        return new GetDepositDetailsResponse()
        {
            Index = deposit.Index,
            Root = root,
            Indices = indices,
            Path = path.Select(node => node.Commitment).ToList(),
        };
    }

    private async Task<string> PopulatePathAndIndicesRecursively(MerkleNode node, List<MerkleNode> path, List<int> indices, CancellationToken cancellationToken)
    {
        if (node.IsRoot) return node.Commitment;

        (long neighbourIndex, int neighbourLayer) = node.CalculateNeighbourCoordinates();

        MerkleNode neighbour = await GetNode(neighbourIndex, neighbourLayer, cancellationToken);
        path.Add(neighbour);
        indices.Add(CalculatePathIndex(neighbour));

        (long parentIndex, int parentLayer) = node.CalculateParentCoordinate();
        MerkleNode parent = await GetNode(parentIndex, parentLayer, cancellationToken);

        return await PopulatePathAndIndicesRecursively(parent, path, indices, cancellationToken);
    }

    private async Task<MerkleNode> GetNode(long index, int layer, CancellationToken cancellationToken)
    {
        MerkleNode? node = await merkleNodeRepository.GetByNodeCoordinate(index, layer, false, cancellationToken);
        if (node is null)
        {
            node = new MerkleNode(layer)
            {
                Index = index,
                Commitment = merkleNodeRepository.GetZeroValue(layer).ToHexForBytes32(),
            };
        }

        return node;
    }

    private static int CalculatePathIndex(MerkleNode neighbour)
    {
        return neighbour.Index % 2 == 0 ? 1 : 0;
    }

    private async Task<MerkleNode> EnsureAndGetDeposit(GetDepositDetailsQuery request, CancellationToken cancellationToken)
    {
        MerkleNode node = (await merkleNodeRepository.GetByCommitment(request.Commitment, true, cancellationToken))!;
        if (node.Layer != 0)
        {
            throw new AppException("Deposit not found", ExceptionCode.OBJECT_NOT_FOUND);
        }

        return node;
    }
}