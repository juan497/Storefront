
using Microsoft.EntityFrameworkCore;
using Storefront.Models;

namespace Storefront.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}
