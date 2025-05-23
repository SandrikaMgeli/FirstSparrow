using FirstSparrow.Application.Features.Deposits.SyncDeposits;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FirstSparrow.Infrastructure.Workers;

public class DepositEventListener(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DepositEventListener> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Starting deposit event listener.");

                await using AsyncServiceScope scope = serviceScopeFactory.CreateAsyncScope();

                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new SyncDepositsCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in deposit event listener.");
            }
            finally
            {
                await Task.Delay(20000, stoppingToken);
            }
        }
    }
}