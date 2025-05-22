using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions.Base;

namespace FirstSparrow.Application.Repositories.Abstractions;

public interface IMerkleNodeRepository : IBaseRepository<MerkleNode, int>
{
    Task<MerkleNode?> GetByNodeCoordinate(uint index, int layer, bool ensureExists, CancellationToken cancellationToken = default);

    Task<MerkleNode> GetNeighbour(MerkleNode merkleNode, CancellationToken cancellationToken = default);
}