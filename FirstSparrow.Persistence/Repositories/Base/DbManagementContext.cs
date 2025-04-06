using Dapper;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManagementContext : IDbManagementContext
{
    internal NpgsqlConnection? Connection { get; set; }

    internal NpgsqlTransaction? Transaction { get; set; }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if(Transaction != null) return Transaction.CommitAsync(cancellationToken);

        throw new InvalidOperationException("Transaction was null and you tried to commit");
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if(Transaction != null) return Transaction.RollbackAsync(cancellationToken);

        throw new InvalidOperationException("Transaction was null and you tried to rollback");
    }

    public async ValueTask DisposeAsync()
    {
        if (Connection != null) await Connection.DisposeAsync();
        if (Transaction != null) await Transaction.DisposeAsync();
        Reset(); // We are resetting it because, if someone calls DisposeAsync more than once on this object, id will do nothing,
        // Because Connection and Transaction will be already null.
    }

    private void Reset()
    {
        Connection = null;
        Transaction = null;
    }
}

public static class DbManagementContextExtensions
{
    /// <summary>
    /// Returns amount of raws effected
    /// </summary>
    public static Task<int> ExecuteAsync(this DbManagementContext context, string sql, object? param)
    {
        return context.Connection!.ExecuteAsync(sql, param, context.Transaction);
    }

    public static Task<T?> QuerySingleOrDefaultAsync<T>(this DbManagementContext context, string sql, object? param)
    {
        return context.Connection!.QuerySingleOrDefaultAsync<T>(sql, param, context.Transaction);
    }

    public static Task<T> QuerySingleAsync<T>(this DbManagementContext context, string sql, object? param)
    {
        return context.Connection!.QuerySingleAsync<T>(sql, param, context.Transaction);
    }
}