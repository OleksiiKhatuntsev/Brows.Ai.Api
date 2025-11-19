using Domain.Db;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests.Infrastructure.Repositories;

/// <summary>
/// Unit tests for PromptRepository
/// </summary>
public class PromptRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_WithExistingPrompts_ReturnsAllPrompts()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new PromptRepository(context);

        var prompt1 = new Prompt { Id = Guid.NewGuid(), Title = "Test Prompt 1", Body = "Body 1" };
        var prompt2 = new Prompt { Id = Guid.NewGuid(), Title = "Test Prompt 2", Body = "Body 2" };

        await context.Prompts.AddRangeAsync(prompt1, prompt2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(prompt1);
        result.Should().Contain(prompt2);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new PromptRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingPrompt_ReturnsPrompt()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new PromptRepository(context);

        var promptId = Guid.NewGuid();
        var prompt = new Prompt { Id = promptId, Title = "Test Prompt", Body = "Test Body" };

        await context.Prompts.AddAsync(prompt);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(promptId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(prompt);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new PromptRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithValidPrompt_AddsAndReturnsPrompt()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new PromptRepository(context);

        var newPrompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "New Prompt",
            Body = "New Body"
        };

        // Act
        var result = await repository.AddAsync(newPrompt);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(newPrompt);
        result.Id.Should().Be(newPrompt.Id);

        var savedPrompt = await context.Prompts.FindAsync(newPrompt.Id);
        savedPrompt.Should().NotBeNull();
        savedPrompt.Should().Be(newPrompt);
    }

    private BrowsAiDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BrowsAiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BrowsAiDbContext(options);
    }
}
