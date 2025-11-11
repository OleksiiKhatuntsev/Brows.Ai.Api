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
    #region Seeding Behavior Tests

    [Fact]
    public async Task SeedAsync_WhenDatabaseIsEmpty_ShouldAddFivePrompts()
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
    public async Task SeedAsync_WhenDatabaseAlreadyHasData_ShouldNotAddMorePrompts()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Add existing data
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

    #endregion

    #region Seed Data Validation Tests

    [Fact]
    public async Task SeedAsync_ShouldSeedPromptsWithUniqueIds()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        var uniqueIds = prompts.Select(p => p.Id).Distinct().ToList();
        uniqueIds.Should().HaveCount(prompts.Count, "all prompt IDs should be unique");
    }

    [Fact]
    public async Task SeedAsync_ShouldSeedPromptsWithNonEmptyTitles()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        prompts.Should().OnlyContain(p => !string.IsNullOrWhiteSpace(p.Title),
            "all prompts should have non-empty titles");
    }

    [Fact]
    public async Task SeedAsync_ShouldSeedPromptsWithNonEmptyBodies()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        prompts.Should().OnlyContain(p => !string.IsNullOrWhiteSpace(p.Body),
            "all prompts should have non-empty bodies");
    }

    [Fact]
    public async Task SeedAsync_AllSeededPrompts_ShouldHaveValidGuids()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        // Act
        await DbSeeder.SeedAsync(context);

        // Assert
        var prompts = await context.Prompts.ToListAsync();
        prompts.Should().OnlyContain(p => p.Id != Guid.Empty,
            "all prompts should have valid (non-empty) GUIDs");
    }

    #endregion

    #region Idempotency Tests

    [Fact]
    public async Task SeedAsync_CalledMultipleTimes_ShouldOnlySeedOnce()
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

    #endregion

    private BrowsAiDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BrowsAiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BrowsAiDbContext(options);
    }

}

