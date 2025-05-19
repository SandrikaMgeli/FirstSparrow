using Dapper;
using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Domain.Extensions;
using FirstSparrow.Application.Repositories.Abstractions.Base;

namespace FirstSparrow.Persistence.Repositories.Base;

public class BaseRepository<TEntity, TId>(
    DbManagementContext context,
    string insertQuery,
    string deleteQuery,
    string updateQuery,
    string getByIdQuery) : IBaseRepository<TEntity, TId>
    where TId : struct, IComparable<TId>, IEquatable<TId>
    where TEntity : BaseEntity<TId>, new()
{
    public async Task Insert(TEntity entity, CancellationToken cancellationToken = default)
    {
        if(context.Connection is null) throw new InvalidOperationException("Database connection was not established.");

        TId result = await context.Connection.QuerySingleAsync<TId>(insertQuery, entity, context.Transaction);
        entity.Id = result;
    }

    public async Task Update(TEntity entity, bool ensureUpdated, CancellationToken cancellationToken = default)
    {
        if(context.Connection is null) throw new InvalidOperationException("Database connection was not established.");

        int effected = await context.Connection.ExecuteAsync(updateQuery, entity, context.Transaction);

        if (ensureUpdated && effected == 0)
        {
            throw new InvalidOperationException($"no rows effected while updating: {typeof(TEntity).FullName}. with id: {entity.Id}.");
        }
    }

    public async Task<TEntity> GetById(TId id, bool ensureExists, CancellationToken cancellationToken = default)
    {
        if(context.Connection is null) throw new InvalidOperationException("Database connection was not established.");

        TEntity? result = await context.Connection.QuerySingleOrDefaultAsync<TEntity>(getByIdQuery, new { id }, context.Transaction);

        if (ensureExists)
        {
            result.EnsureExists($"id: {id}");
        }

        return result!;
    }

    public async Task Delete(TEntity entity, bool ensureDeleted, CancellationToken cancellationToken = default)
    {
        if(context.Connection is null) throw new InvalidOperationException("Database connection was not established.");

        int effected = await context.Connection.ExecuteAsync(deleteQuery, entity, context.Transaction);

        if (ensureDeleted && effected == 0)
        {
            throw new InvalidOperationException($"no rows effected while deleting: {typeof(TEntity).FullName}. with id: {entity.Id}.");
        }
    }
}