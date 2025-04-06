namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IDbManagementContext : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}