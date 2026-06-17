using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Files.Persistence;

public class FilesDbContext : DbContext
{
    public FilesDbContext(DbContextOptions<FilesDbContext> options) : base(options) { }

    public DbSet<MediaBucket> Buckets => Set<MediaBucket>();
    public DbSet<FileRecord> Files => Set<FileRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesDbContext).Assembly);
    }
}
