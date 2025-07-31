using Microsoft.EntityFrameworkCore;
using Venice.Domain.Entities;
using Venice.Infra.Configurations;

namespace Venice.Infra.Data;

public class VeniceDbContext(DbContextOptions<VeniceDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("venice");
        modelBuilder.ApplyConfiguration(new OrderConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
