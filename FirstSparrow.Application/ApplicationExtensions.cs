using System.Reflection;
using FirstSparrow.Application.Services;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeProvider = System.TimeProvider;

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

        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IOtpService, OtpService>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptions<IdentitySettings>()
            .BindConfiguration(nameof(IdentitySettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}