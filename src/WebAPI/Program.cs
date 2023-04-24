using Microsoft.EntityFrameworkCore;
using WebAPI.Infra.Middlewares;
using WebAPI.Infra.Persistence;
using WebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

// Register your IDatabaseConfiguration implementation.
services.AddSingleton<IDatabaseConfiguration, PostgreSqlDatabaseConfiguration>();

// Configure the tenant registry database.
services.AddDbContext<TenantRegistryDbContext>((serviceProvider, options) =>
{
    var config = serviceProvider.GetRequiredService<IDatabaseConfiguration>();
    config.Configure(options, builder.Configuration.GetConnectionString("TenantRegistry"));
});

// Configure the tenant-specific databases.
services.AddDbContext<TenantSpecificDbContext>((serviceProvider, options) =>
{
    var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
    string connectionString;

    if (httpContextAccessor?.HttpContext?.Items["Tenant"] is Tenant tenant)
    {
        connectionString = tenant.ConnectionString;
    }
    else
    {
        // Replace with your default connection string for design-time migrations.
        // You can also retrieve it from a configuration file or an environment variable.
        connectionString = "Host=localhost;Database=TenantSpecific;Username=postgres;Password=root";
    }

    var config = serviceProvider.GetRequiredService<IDatabaseConfiguration>();
    config.Configure(options, connectionString);
});

// Add HttpContextAccessor to access HttpContext outside controllers.
services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Create a scope to resolve scoped services.
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var tenantRegistryDbContext = serviceProvider.GetRequiredService<TenantRegistryDbContext>();
    tenantRegistryDbContext.Database.Migrate();
}

// Add the tenant resolver middleware to the pipeline.
app.UseMiddleware<TenantResolverMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
