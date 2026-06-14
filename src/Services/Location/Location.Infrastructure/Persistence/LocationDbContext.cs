using Location.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Infrastructure.Persistence;

public class LocationDbContext(DbContextOptions<LocationDbContext> options) : DbContext(options)
{
    public DbSet<UserLocation> UserLocations => Set<UserLocation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserLocation>(e =>
        {
            e.HasKey(l => l.Id);
            e.HasIndex(l => l.UserId);
            e.HasIndex(l => new { l.Latitude, l.Longitude });
            e.Property(l => l.Label).HasMaxLength(100);
        });
    }
}
