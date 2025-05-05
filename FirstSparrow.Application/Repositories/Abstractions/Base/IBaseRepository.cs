using FirstSparrow.Application.Domain.Entities.Base;

namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IBaseRepository<TEntity, in TId>
    where TId : struct
    where TEntity : BaseEntity<TId>, new()
{
    Task Insert(TEntity restaurant, CancellationToken cancellationToken = default);

    Task Update(TEntity restaurant, CancellationToken cancellationToken = default);

    Task<TEntity?> GetById(TId id, bool ensureExists, CancellationToken cancellationToken = default);

    Task Delete(TId id, CancellationToken cancellationToken = default);
}