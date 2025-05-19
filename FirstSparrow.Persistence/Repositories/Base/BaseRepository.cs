using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Repositories.Abstractions.Base;

namespace FirstSparrow.Persistence.Repositories.Base;

public class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId>
    where TId : struct
    where TEntity : BaseEntity<TId>, new()
{
    public async Task<TId> Insert(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Update(TEntity entity, bool ensureUpdated, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> GetById(TId id, bool ensureExists, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(TEntity entity, bool ensureDeleted, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}