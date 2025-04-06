using FirstSparrow.Application.Repositories.Abstractions.Base;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManager(
    DbManagementContext context, 
    ConnectionStringProvider connectionStringProvider) : IDbManager
{
    public async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        await using (context.Connection = new NpgsqlConnection(connectionStringProvider.ConnectionString))
        {
            await context.Connection.OpenAsync(cancellationToken);
            T result = await operation(cancellationToken);
            return result;
        }
    }

    public async Task<T> RunWithTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        await using (context.Connection = new NpgsqlConnection(connectionStringProvider.ConnectionString))
        {
            await context.Connection.OpenAsync(cancellationToken);
            await using (context.Transaction = await context.Connection.BeginTransactionAsync(cancellationToken))
            {
                T result = await operation(cancellationToken);
                return result;
            }
        }
    }

    public async Task RunAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        await using (context.Connection = new NpgsqlConnection(connectionStringProvider.ConnectionString))
        {
            await context.Connection.OpenAsync(cancellationToken);
            await operation(cancellationToken);
        }
    }

    public async Task RunWithTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        await using (context.Connection = new NpgsqlConnection(connectionStringProvider.ConnectionString))
        {
            await context.Connection.OpenAsync(cancellationToken);
            await using (context.Transaction = await context.Connection.BeginTransactionAsync(cancellationToken))
            {
                await operation(cancellationToken);
            }
        }
    }
}