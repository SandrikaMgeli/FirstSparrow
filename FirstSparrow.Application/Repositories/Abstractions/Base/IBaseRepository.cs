using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IBaseRepository<TEntity, TId>
    where TId : struct, IComparable<TId>, IEquatable<TId>
    where TEntity : BaseEntity<TId>
{
    Task Insert(TEntity entity, CancellationToken cancellationToken = default);

    Task Update(TEntity entity, bool ensureUpdated, CancellationToken cancellationToken = default);

    Task<TEntity> GetById(TId id, bool ensureExists, CancellationToken cancellationToken = default);

    Task Delete(TId id, bool ensureDeleted, CancellationToken cancellationToken = default);
}