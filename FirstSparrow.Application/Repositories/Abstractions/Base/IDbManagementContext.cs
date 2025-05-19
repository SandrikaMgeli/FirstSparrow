namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IDbManagementContext : IAsyncDisposable, IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}