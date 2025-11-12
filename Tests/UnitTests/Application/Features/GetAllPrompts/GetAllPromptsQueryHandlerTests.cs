using Application.Features.GetAllPrompts;
using Domain.Db;
using Domain.Interfaces.Infrastructure;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests.Application.Features.GetAllPrompts;

/// <summary>
/// Unit tests for GetAllPromptsQueryHandler
/// </summary>
public class GetAllPromptsQueryHandlerTests
{
    private readonly Mock<IPromptRepository> _mockRepository;
    private readonly GetAllPromptsQueryHandler _handler;

    public GetAllPromptsQueryHandlerTests()
    {
        _mockRepository = new Mock<IPromptRepository>();
        _handler = new GetAllPromptsQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenPromptsExist_ShouldReturnAllPrompts()
    {
        // Arrange
        var expectedPrompts = new List<Prompt>
        {
            new() { Id = Guid.NewGuid(), Title = "Prompt 1", Body = "Body 1" },
            new() { Id = Guid.NewGuid(), Title = "Prompt 2", Body = "Body 2" },
            new() { Id = Guid.NewGuid(), Title = "Prompt 3", Body = "Body 3" }
        };

        _mockRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedPrompts);

        var query = new GetAllPromptsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(expectedPrompts);
    }

    [Fact]
    public async Task Handle_WhenNoPromptsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyList = new List<Prompt>();

        _mockRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(emptyList);

        var query = new GetAllPromptsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryGetAllAsyncOnce()
    {
        // Arrange
        var prompts = new List<Prompt>
        {
            new() { Id = Guid.NewGuid(), Title = "Test", Body = "Test Body" }
        };

        _mockRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(prompts);

        var query = new GetAllPromptsQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultiplePrompts_ShouldReturnAllInCorrectOrder()
    {
        // Arrange
        var prompt1 = new Prompt { Id = Guid.NewGuid(), Title = "First", Body = "Body 1" };
        var prompt2 = new Prompt { Id = Guid.NewGuid(), Title = "Second", Body = "Body 2" };
        var prompt3 = new Prompt { Id = Guid.NewGuid(), Title = "Third", Body = "Body 3" };

        var expectedPrompts = new List<Prompt> { prompt1, prompt2, prompt3 };

        _mockRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedPrompts);

        var query = new GetAllPromptsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().ContainInOrder(prompt1, prompt2, prompt3);
    }
}

