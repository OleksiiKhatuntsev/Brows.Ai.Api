using Domain.Db;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.Infrastructure.Data;

/// <summary>
/// Tests for BrowsAiDbContext to verify database constraints and validation are enforced.
/// </summary>
public class BrowsAiDbContextTests
{
    [Fact]
    public async Task SaveChangesAsync_WithNullTitle_ThrowsValidationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = null!,
            Body = "Valid Body"
        };

        // Act
        context.Prompts.Add(prompt);
        var act = async () => await context.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
            .WithMessage("*Title*required*");
    }

    [Fact]
    public async Task SaveChangesAsync_WithNullBody_ThrowsValidationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Body = null!
        };

        // Act
        context.Prompts.Add(prompt);
        var act = async () => await context.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
            .WithMessage("*Body*required*");
    }

    [Fact]
    public async Task SaveChangesAsync_WithValidPrompt_Succeeds()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Body = "Valid Body"
        };

        // Act
        context.Prompts.Add(prompt);
        var saveResult = async () => await context.SaveChangesAsync();

        // Assert
        await saveResult.Should().NotThrowAsync();

        var savedPrompt = await context.Prompts.FindAsync(prompt.Id);
        savedPrompt.Should().NotBeNull();
        savedPrompt!.Title.Should().Be("Valid Title");
        savedPrompt.Body.Should().Be("Valid Body");
    }

    [Fact]
    public async Task Add_WithDuplicateId_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var id = Guid.NewGuid();

        var prompt1 = new Prompt
        {
            Id = id,
            Title = "First Prompt",
            Body = "First Body"
        };

        var prompt2 = new Prompt
        {
            Id = id,
            Title = "Second Prompt",
            Body = "Second Body"
        };

        // Act
        context.Prompts.Add(prompt1);
        await context.SaveChangesAsync();

        var act = () => context.Prompts.Add(prompt2);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*tracked*");
    }

    [Fact]
    public async Task AddRange_WithUniqueIds_Succeeds()
    {
        // Arrange
        await using var context = CreateInMemoryContext();

        var prompt1 = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "First Prompt",
            Body = "First Body"
        };

        var prompt2 = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "Second Prompt",
            Body = "Second Body"
        };

        // Act
        context.Prompts.AddRange(prompt1, prompt2);
        var saveResult = async () => await context.SaveChangesAsync();

        // Assert
        await saveResult.Should().NotThrowAsync();

        var allPrompts = await context.Prompts.ToListAsync();
        allPrompts.Should().HaveCount(2);
    }

    [Fact]
    public async Task Add_WithValidPrompt_AddsToDatabase()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var promptId = Guid.NewGuid();
        var prompt = new Prompt
        {
            Id = promptId,
            Title = "Test Title",
            Body = "Test Body"
        };

        // Act
        context.Prompts.Add(prompt);
        await context.SaveChangesAsync();

        // Assert
        var savedPrompt = await context.Prompts.FindAsync(promptId);
        savedPrompt.Should().NotBeNull();
        savedPrompt!.Id.Should().Be(promptId);
        savedPrompt.Title.Should().Be("Test Title");
        savedPrompt.Body.Should().Be("Test Body");
    }

    [Fact]
    public async Task Find_WithExistingId_RetrievesPrompt()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var promptId = Guid.NewGuid();
        var prompt = new Prompt
        {
            Id = promptId,
            Title = "Test Title",
            Body = "Test Body"
        };
        context.Prompts.Add(prompt);
        await context.SaveChangesAsync();

        // Act
        var retrieved = await context.Prompts.FindAsync(promptId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(promptId);
        retrieved.Title.Should().Be("Test Title");
        retrieved.Body.Should().Be("Test Body");
    }

    [Fact]
    public async Task Update_WithExistingPrompt_UpdatesSuccessfully()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var promptId = Guid.NewGuid();
        var prompt = new Prompt
        {
            Id = promptId,
            Title = "Original Title",
            Body = "Original Body"
        };
        context.Prompts.Add(prompt);
        await context.SaveChangesAsync();

        // Act - Since Prompt uses init-only properties, update requires replace
        var existingPrompt = await context.Prompts.FindAsync(promptId);
        context.Prompts.Remove(existingPrompt!);

        var updatedPrompt = new Prompt
        {
            Id = promptId,
            Title = "Updated Title",
            Body = "Updated Body"
        };
        context.Prompts.Add(updatedPrompt);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.Prompts.FindAsync(promptId);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Body.Should().Be("Updated Body");
    }

    [Fact]
    public async Task Remove_WithExistingPrompt_DeletesFromDatabase()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var promptId = Guid.NewGuid();
        var prompt = new Prompt
        {
            Id = promptId,
            Title = "Test Title",
            Body = "Test Body"
        };
        context.Prompts.Add(prompt);
        await context.SaveChangesAsync();

        // Act
        var promptToDelete = await context.Prompts.FindAsync(promptId);
        context.Prompts.Remove(promptToDelete!);
        await context.SaveChangesAsync();

        // Assert
        var deleted = await context.Prompts.FindAsync(promptId);
        deleted.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithOptions_InitializesSuccessfully()
    {
        // Arrange & Act
        var act = () => CreateInMemoryContext();

        // Assert
        act.Should().NotThrow();

        using var context = act();
        context.Should().NotBeNull();
        context.Prompts.Should().NotBeNull();
    }

    private BrowsAiDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BrowsAiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BrowsAiDbContext(options);
    }
}
