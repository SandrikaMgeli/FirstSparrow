using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Application.Repositories.Abstractions.Base;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Persistence.Repositories;
using FirstSparrow.Persistence.Repositories.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        services.AddScoped<IMetadataRepository, MetadataRepository>();
        services.AddScoped<IMerkleNodeRepository, MerkleNodeRepository>();

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
        IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();
        metadataService.InitializeNecessaryMetadata().GetAwaiter().GetResult();
        return host;
    }
}