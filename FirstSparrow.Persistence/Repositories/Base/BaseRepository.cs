using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Domain.Extensions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using Microsoft.EntityFrameworkCore;

namespace FirstSparrow.Persistence.Repositories.Base;

public class BaseRepository<TEntity, TId>(
    FirstSparrowDbContext dbContext) : IBaseRepository<TEntity, TId>
    where TId : struct
    where TEntity : BaseEntity<TId>, new()
{
    public async Task<TId> Insert(TEntity entity, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Entry(entity).State = EntityState.Detached;
        return entity.Id;
    }

    public async Task Update(TEntity entity, bool ensureUpdated, CancellationToken cancellationToken = default)
    {
        dbContext.Update(entity);
        int effected = await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Entry(entity).State = EntityState.Detached;

        if (ensureUpdated && effected == 0)
        {
            throw new AppException($"{typeof(TEntity)} not found with id: {entity.Id}", ExceptionCode.OBJECT_NOT_FOUND);
        }
    }

    public async Task<TEntity> GetById(TId id, bool ensureExists, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await dbContext.Set<TEntity>().SingleOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken);

        if (ensureExists)
        {
            entity.EnsureExists($"Id = {id}");
        }

        return entity!;
    }

    public async Task Delete(TEntity entity, bool ensureDeleted, CancellationToken cancellationToken = default)
    {
        dbContext.Remove(entity);
        int effected = await dbContext.SaveChangesAsync(cancellationToken);

        if (ensureDeleted && effected == 0)
        {
            throw new AppException($"{typeof(TEntity)} not found with id: {entity.Id}", ExceptionCode.OBJECT_NOT_FOUND);
        }
    }
}