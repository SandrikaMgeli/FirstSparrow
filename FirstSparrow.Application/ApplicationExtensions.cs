using System.Reflection;
using FirstSparrow.Application.Services;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FirstSparrow.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddOptions();

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<RequestMetadata>();

        // Services
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IMetadataService, MetadataService>();
        services.AddScoped<IDepositService, DepositService>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptions<FirstSparrowConfigs>()
            .BindConfiguration("FirstSparrowConfigs")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}