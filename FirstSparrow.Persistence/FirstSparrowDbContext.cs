using FirstSparrow.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FirstSparrow.Persistence;

public class FirstSparrowDbContext(
    IOptions<ConnectionStringProvider> connectionStringOptions) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionStringOptions.Value.ConnectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}