using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Services.Models;
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
        logger.LogInformation("Starting deposit event listener.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                IBlockChainService blockChainService = scope.ServiceProvider.GetRequiredService<IBlockChainService>();
                await blockChainService.FetchDeposits(new FetchDepositsParams()
                {
                    FromBlock = 8312000,
                    BatchSize = 500,
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in deposit event listener.");
            }
            finally
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}