using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Shared;
using FirstSparrow.Infrastructure.Services;
using FirstSparrow.Infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FirstSparrow.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // HttpClients
        services.AddHttpClient("ethereum_rpc", (serviceProvider, httpClient) =>
        {
            IOptions<FirstSparrowConfigs> applicationConfigs = serviceProvider.GetRequiredService<IOptions<FirstSparrowConfigs>>();
            httpClient.BaseAddress = new Uri(applicationConfigs.Value.RpcUrl);
        });

        // Services
        services.AddScoped<IBlockChainService, EthereumService>();

        // Workers
        services.AddHostedService<DepositEventListener>();

        return services;
    }
}