namespace FirstSparrow.Application.Repositories.Abstractions.Base;

public interface IDbManager
{
    // Task<T> RunAsync<T>(
    //     Func<CancellationToken, Task<T>> operation, 
    //     CancellationToken cancellationToken = default);
    //
    // Task<T> RunWithTransactionAsync<T>(
    //     Func<CancellationToken, Task<T>> operation,
    //     CancellationToken cancellationToken = default);
    //
    // Task RunAsync(
    //     Func<CancellationToken, Task> operation, 
    //     CancellationToken cancellationToken = default);
    //
    // Task RunWithTransactionAsync(
    //     Func<CancellationToken, Task> operation, 
    //     CancellationToken cancellationToken = default);

    Task<IDbManagementContext> RunAsync(CancellationToken cancellationToken = default);

    Task<IDbManagementContext> RunWithTransactionAsync(CancellationToken cancellationToken = default);
}