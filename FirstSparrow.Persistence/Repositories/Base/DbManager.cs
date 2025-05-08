using System.Data;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManager(
    DbManagementContext context, 
    IOptions<ConnectionStringProvider> connectionStringProvider) : IDbManager
{
    public async Task<IDbManagementContext> RunAsync(CancellationToken cancellationToken = default)
    {
        context.Connection = new NpgsqlConnection(connectionStringProvider.Value.ConnectionString);
        await context.Connection.OpenAsync(cancellationToken);
        return context;
    }

    public async Task<IDbManagementContext> RunWithTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        context.Connection = new NpgsqlConnection(connectionStringProvider.Value.ConnectionString);
        await context.Connection.OpenAsync(cancellationToken);
        context.Transaction = await context.Connection.BeginTransactionAsync(isolationLevel, cancellationToken);
        return context;
    }
}