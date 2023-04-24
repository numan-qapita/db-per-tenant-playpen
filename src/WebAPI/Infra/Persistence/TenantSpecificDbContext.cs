using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;
using WebAPI.Models;

namespace WebAPI.Infra.Persistence;

public class TenantSpecificDbContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<Customer> Customers { get; set; }

    public TenantSpecificDbContext(DbContextOptions<TenantSpecificDbContext> options)
        : base(options)
    {
    }

    public TenantSpecificDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var nameTranslator = new NpgsqlSnakeCaseNameTranslator();
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(nameTranslator.TranslateTypeName(entityType.GetTableName()));

            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(nameTranslator.TranslateMemberName(property.GetColumnName()));
            }
        }
    }
}
