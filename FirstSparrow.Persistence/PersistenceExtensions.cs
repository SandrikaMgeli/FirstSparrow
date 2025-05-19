using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FirstSparrow.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FirstSparrowDbContext>();

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

    public static IHost SyncDatabase(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        DbContext context = scope.ServiceProvider.GetRequiredService<FirstSparrowDbContext>();
        context.Database.Migrate();
        return host;
    }
}