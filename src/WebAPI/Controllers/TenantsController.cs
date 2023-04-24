using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infra.Persistence;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly TenantRegistryDbContext _dbContext;
    private readonly IDatabaseConfiguration _databaseConfiguration;

    public TenantsController(TenantRegistryDbContext dbContext, IDatabaseConfiguration databaseConfiguration)
    {
        _dbContext = dbContext;
        _databaseConfiguration = databaseConfiguration;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] Tenant tenant)
    {
        // Add the tenant to the registry.
        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        // Create the tenant-specific database.
        var optionsBuilder = new DbContextOptionsBuilder<TenantSpecificDbContext>();
        _databaseConfiguration.Configure(optionsBuilder, tenant.ConnectionString);

        using var tenantDbContext = new TenantSpecificDbContext(optionsBuilder.Options);
        await tenantDbContext.Database.EnsureCreatedAsync();

        return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
    }

    [HttpGet("{id}")]
    public IActionResult GetTenant(int id)
    {
        var tenant = _dbContext.Tenants.SingleOrDefault(t => t.Id == id);
        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }
}
