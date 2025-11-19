using Domain.Db;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.Infrastructure.Data;

/// <summary>
/// Tests for DbSeeder to verify seeding logic and data integrity.
/// </summary>
public class DbSeederTests
{
    [Fact]
    public async Task SeedAsync_WithEmptyDatabase_AddsFivePrompts()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        prompts.Should().HaveCount(5);
    }

    [Fact]
    public async Task SeedAsync_WithExistingData_DoesNotAddMorePrompts()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        var existingPrompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Existing Prompt",
            Body = "Existing Body"
        };
        context.Prompts.Add(existingPrompt);
        await context.SaveChangesAsync();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        prompts.Should().HaveCount(1, "seeding should not occur when data already exists");
        prompts.First().Should().Be(existingPrompt);
    }

    [Fact]
    public async Task SeedAsync_WithValidData_CreatesPromptsWithUniqueIds()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        var uniqueIds = prompts.Select(p => p.Id).Distinct().ToList();
        uniqueIds.Should().HaveCount(prompts.Count, "all prompt IDs should be unique");
        prompts.Should().OnlyContain(p => p.Id != Guid.Empty, "all prompts should have valid GUIDs");
    }

    [Fact]
    public async Task SeedAsync_WithValidData_CreatesPromptsWithNonEmptyContent()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        prompts.Should().OnlyContain(p => !string.IsNullOrWhiteSpace(p.Title),
            "all prompts should have non-empty titles");
        prompts.Should().OnlyContain(p => !string.IsNullOrWhiteSpace(p.Body),
            "all prompts should have non-empty bodies");
    }

    [Fact]
    public async Task SeedAsync_CalledMultipleTimes_IsIdempotent()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);
        await DbSeeder.SeedAsync(context);
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        prompts.Should().HaveCount(5, "seeding should only happen once, regardless of how many times it's called");
    }

    private BrowsAiDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BrowsAiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BrowsAiDbContext(options);
    }
}
