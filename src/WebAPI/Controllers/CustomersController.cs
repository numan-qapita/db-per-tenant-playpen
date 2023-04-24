using Microsoft.AspNetCore.Mvc;
using WebAPI.Infra.Persistence;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/{tenantId:long}/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly TenantSpecificDbContext _dbContext;

    public CustomersController(TenantSpecificDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public IActionResult CreateCustomer([FromBody] Customer customer)
    {
        _dbContext.Customers.Add(customer);
        _dbContext.SaveChanges();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    [HttpGet("{id}")]
    public IActionResult GetCustomer(int id)
    {
        var customer = _dbContext.Customers.SingleOrDefault(c => c.Id == id);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    // Implement other actions for updating and deleting customers as needed.
}
