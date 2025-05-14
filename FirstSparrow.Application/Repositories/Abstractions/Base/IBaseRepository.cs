using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IBaseRepository<TEntity, TId>
    where TId : struct
    where TEntity : BaseEntity<TId>, new()
{
    Task<TId> Insert(TEntity entity, CancellationToken cancellationToken = default);

    Task Update(TEntity entity, bool ensureUpdated, CancellationToken cancellationToken = default);

    Task<TEntity> GetById(TId id, bool ensureExists, CancellationToken cancellationToken = default);

    Task Delete(TEntity entity, bool ensureDeleted, CancellationToken cancellationToken = default);
}