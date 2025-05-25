using Dapper;
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
        services.AddScoped<PostgresManagementContext>();
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
        CreateDatabase(scope).GetAwaiter().GetResult();
        IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();
        metadataService.InitializeNecessaryMetadata().GetAwaiter().GetResult();
        return host;
    }

    private static async Task CreateDatabase(IServiceScope scope)
    {
        IDbManager dbManager = scope.ServiceProvider.GetRequiredService<IDbManager>();
        await using PostgresManagementContext context = (PostgresManagementContext)dbManager.RunAsync().GetAwaiter().GetResult();
        List<string> dbCreationScripts = GetDatabaseScripts();
        await RunDbCreationScripts(dbCreationScripts, context);
    }

    private static async Task RunDbCreationScripts(List<string> dbCreationScripts, PostgresManagementContext context)
    {
        foreach (string script in dbCreationScripts)
        {
            await context.Connection!.ExecuteAsync(script);
        }
    }

    private static List<string> GetDatabaseScripts()
    {
        string dbFilesPath = Path.Combine(AppContext.BaseDirectory, "Db");

        if (!Directory.Exists(dbFilesPath))
        {
            throw new Exception("Database script creation directory not found");
        }
        string[] dbCreationFiles = Directory.GetFiles(dbFilesPath, "*.sql");
        return dbCreationFiles.Select(File.ReadAllText).ToList();
    }
}