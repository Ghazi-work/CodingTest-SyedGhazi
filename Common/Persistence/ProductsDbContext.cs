using Microsoft.EntityFrameworkCore;
using ProductsApi.Common.Models;

namespace ProductsApi.Common.Persistence;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}
