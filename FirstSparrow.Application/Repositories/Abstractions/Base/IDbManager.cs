using System.Data;

namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IDbManager
{
    Task<IDbManagementContext> RunAsync(CancellationToken cancellationToken = default);

    Task<IDbManagementContext> RunWithTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
}