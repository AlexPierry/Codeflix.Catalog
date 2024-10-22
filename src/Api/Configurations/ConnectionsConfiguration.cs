using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbConnection(configuration);
        return services;
    }

    private static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");
        services.AddDbContext<CodeflixCatalogDbContext>(
            options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        return services;
    }
}