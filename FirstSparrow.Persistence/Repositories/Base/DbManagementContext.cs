using FirstSparrow.Application.Repositories.Abstractions.Base;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManagementContext : IDbManagementContext
{
    internal NpgsqlTransaction?  Transaction { get; set; }

    internal NpgsqlConnection?  Connection { get; set; }

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
        if(Connection != null) await Connection.DisposeAsync();
        if (Transaction != null) await Transaction.DisposeAsync();
        Reset(); 
        /* We are resetting it because, if someone calls DisposeAsync more than once on this object, it will do nothing,
        Because Connection and Transaction will be already null.*/
    }

    private void Reset()
    {
        Transaction = null;
        Connection = null;
    }
}