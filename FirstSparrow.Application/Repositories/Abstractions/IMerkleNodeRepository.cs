using System.Numerics;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions.Base;

namespace FirstSparrow.Application.Repositories.Abstractions;

public interface IMerkleNodeRepository : IBaseRepository<MerkleNode, int>
{
    Task<MerkleNode?> GetByNodeCoordinate(long index, int layer, bool ensureExists, CancellationToken cancellationToken = default);

    Task<MerkleNode> GetNeighbour(MerkleNode merkleNode, CancellationToken cancellationToken = default);

    Task<MerkleNode?> GetByCommitment(string commitment, bool ensureExists, CancellationToken cancellationToken = default);

    BigInteger GetZeroValue(int layer);
}