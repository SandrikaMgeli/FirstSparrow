namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IDbManager
{
    Task<IDbManagementContext> RunAsync(CancellationToken cancellationToken = default);

    Task<IDbManagementContext> RunWithTransactionAsync(CancellationToken cancellationToken = default);
}