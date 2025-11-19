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
    public async Task Handle_WithExistingPrompts_ReturnsAllPrompts()
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
        result.Should().ContainInOrder(expectedPrompts);
        _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoPrompts_ReturnsEmptyList()
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
        _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}
