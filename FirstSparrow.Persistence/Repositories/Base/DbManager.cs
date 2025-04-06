using FirstSparrow.Application.Repositories.Abstractions.Base;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManager : IDbManager
{
    private readonly DbManagementContext _context;
    private readonly ConnectionStringProvider _connectionStringProvider;

    public DbManager(DbManagementContext context, ConnectionStringProvider connectionStringProvider)
    {
        _context = context;
        _connectionStringProvider = connectionStringProvider;
    }

    public async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        await using (_context.Connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString))
        {
            await _context.Connection.OpenAsync(cancellationToken);
            T result = await operation(cancellationToken);
            return result;
        }
    }

    public async Task<T> RunWithTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        await using (_context.Connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString))
        {
            await _context.Connection.OpenAsync(cancellationToken);
            await using (_context.Transaction = await _context.Connection.BeginTransactionAsync(cancellationToken))
            {
                T result = await operation(cancellationToken);
                return result;
            }
        }
    }

    public async Task RunAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        await using (_context.Connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString))
        {
            await _context.Connection.OpenAsync(cancellationToken);
            await operation(cancellationToken);
        }
    }

    public async Task RunWithTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        await using (_context.Connection = new NpgsqlConnection(_connectionStringProvider.ConnectionString))
        {
            await _context.Connection.OpenAsync(cancellationToken);
            await using (_context.Transaction = await _context.Connection.BeginTransactionAsync(cancellationToken))
            {
                await operation(cancellationToken);
            }
        }
    }
}