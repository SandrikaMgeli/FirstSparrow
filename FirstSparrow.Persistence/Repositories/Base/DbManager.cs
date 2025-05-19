using System.Data;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManager(
    DbManagementContext context,
    IOptions<ConnectionStringProvider> connectionStringProvider,
    ILogger<DbManager> logger) : IDbManager
{
    public async Task<IDbManagementContext> RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            context.Connection = new NpgsqlConnection(connectionStringProvider.Value.ConnectionString);
            await context.Connection.OpenAsync(cancellationToken);
            return context;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while trying to connect to the database");
            throw;
        }
    }

    public async Task<IDbManagementContext> RunWithTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        try
        {
            context.Connection = new NpgsqlConnection(connectionStringProvider.Value.ConnectionString);
            await context.Connection.OpenAsync(cancellationToken);
            context.Transaction = await context.Connection.BeginTransactionAsync(isolationLevel, cancellationToken);
            return context;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while trying to connect to the database with transaction");
            throw;
        }
    }
}