using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Npgsql.NameTranslation;
using WebAPI.Models;

namespace WebAPI.Infra.Persistence;

public class TenantRegistryDbContext : DbContext
{
    public DbSet<Tenant> Tenants { get; set; }

    public TenantRegistryDbContext(DbContextOptions<TenantRegistryDbContext> options)
        : base(options)
    {
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