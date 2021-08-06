using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpticLtd.Data.Entities;

namespace OpticLtd.Data
{
  public class AppDbContext : IdentityDbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { 

    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductFeature> ProductFeatures { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
  }
}
