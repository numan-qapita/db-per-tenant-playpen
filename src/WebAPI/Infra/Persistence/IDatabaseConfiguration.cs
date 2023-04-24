using Microsoft.EntityFrameworkCore;

namespace WebAPI.Infra.Persistence;

public interface IDatabaseConfiguration
{
    void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString);
}
