using Microsoft.EntityFrameworkCore;

namespace WebAPI.Infra.Persistence;

public class PostgreSqlDatabaseConfiguration : IDatabaseConfiguration
{
    public void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }
}
