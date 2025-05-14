using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Persistence.Repositories.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FirstSparrow.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        //Base
        services.AddScoped<DbManagementContext>();
        services.AddScoped<IDbManager, DbManager>();

        services.AddOptions();

        //Repositories


        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptions<ConnectionStringProvider>()
            .BindConfiguration("FirstSparrowDb")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }
}