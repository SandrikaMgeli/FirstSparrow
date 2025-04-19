namespace FirstSparrow.Api;

public static class ApiExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        return services;
    }
}