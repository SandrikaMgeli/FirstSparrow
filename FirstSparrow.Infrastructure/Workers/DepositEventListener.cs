using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FirstSparrow.Infrastructure.Workers;

public class DepositEventListener(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DepositEventListener> logger) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting deposit event listener.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (IServiceScope scope = serviceScopeFactory.CreateScope())
                {
                    
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in deposit event listener.");
            }
        }
    }
}