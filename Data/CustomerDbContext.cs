using Microsoft.EntityFrameworkCore;
using Storefront.Models;

namespace Storefront.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
}
