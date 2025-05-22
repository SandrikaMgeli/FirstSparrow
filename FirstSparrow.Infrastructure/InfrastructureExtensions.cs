using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Infrastructure.Services.Ethereum;
using FirstSparrow.Infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FirstSparrow.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // HttpClients
        services.AddHttpClient("ethereum_rpc");

        // Services
        services.AddScoped<IBlockChainService, EthereumService>();

        // Workers
        //services.AddHostedService<DepositEventListener>();

        return services;
    }
}