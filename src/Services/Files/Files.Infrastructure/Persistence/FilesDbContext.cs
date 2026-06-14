using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Files.Infrastructure.Persistence;

public class FilesDbContext(DbContextOptions<FilesDbContext> options) : DbContext(options)
{
    public DbSet<UserFile> UserFiles => Set<UserFile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserFile>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.OriginalName).HasMaxLength(512).IsRequired();
            e.Property(f => f.StoredName).HasMaxLength(256).IsRequired();
            e.Property(f => f.ContentType).HasMaxLength(100).IsRequired();
            e.Property(f => f.StoragePath).HasMaxLength(2048).IsRequired();
            e.HasIndex(f => f.UserId);
        });
    }
}
