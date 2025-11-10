using Domain.Db;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class BrowsAiDbContext : DbContext
{
    public BrowsAiDbContext(DbContextOptions<BrowsAiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Prompt> Prompts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Prompt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Body).IsRequired();
        });
    }
}

