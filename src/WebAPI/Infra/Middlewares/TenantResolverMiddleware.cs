using WebAPI.Infra.Persistence;

namespace WebAPI.Infra.Middlewares;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolverMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TenantRegistryDbContext dbContext)
    {
        var tenantId = context.Request.RouteValues["tenantId"]?.ToString();;

        if (!string.IsNullOrEmpty(tenantId) && long.TryParse(tenantId, out var id))
        {
            // Retrieve tenant from the registry and store it in the HttpContext.
            var tenant = dbContext.Tenants.SingleOrDefault(t => t.Id == id);
            if (tenant != null)
            {
                context.Items["Tenant"] = tenant;
            }
        }

        // Call the next middleware in the pipeline.
        await _next(context);
    }
}
