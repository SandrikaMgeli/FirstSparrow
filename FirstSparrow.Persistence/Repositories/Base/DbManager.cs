using System.Data;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using Microsoft.Extensions.Options;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManager(
    DbManagementContext context, 
    IOptions<ConnectionStringProvider> connectionStringProvider,
    FirstSparrowDbContext dbContext) : IDbManager
{
    public Task<IDbManagementContext> RunAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IDbManagementContext>(context);
    }

    public async Task<IDbManagementContext> RunWithTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        context.Transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return context;
    }
}