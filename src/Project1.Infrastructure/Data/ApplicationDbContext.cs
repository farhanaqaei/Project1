using Microsoft.EntityFrameworkCore;
using Project1.Core.ProductAggregate.Entities;

namespace Project1.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}
