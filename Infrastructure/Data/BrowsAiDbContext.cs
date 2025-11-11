using System.ComponentModel.DataAnnotations;
using Domain.Db;
using Domain.Interfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class BrowsAiDbContext : DbContext, IBrowsAiDbContext
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

    public override int SaveChanges()
    {
        ValidateEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ValidateEntities()
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var entity in entities)
        {
            // Validate using DataAnnotations attributes
            var validationContext = new ValidationContext(entity);
            Validator.ValidateObject(entity, validationContext, validateAllProperties: true);

            // Additional custom validation for Guid.Empty
            if (entity is Prompt prompt && prompt.Id == Guid.Empty)
            {
                throw new ValidationException("Prompt Id cannot be an empty Guid.");
            }
        }
    }
}

