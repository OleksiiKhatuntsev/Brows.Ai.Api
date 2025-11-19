using Application.Features.UpdatePrompt;
using Domain.Db;
using Domain.Interfaces.Application;
using Domain.Interfaces.Infrastructure;
using Domain.Models.Api;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests.Application.Features.UpdatePrompt;

/// <summary>
/// Unit tests for UpdatePromptCommandHandler
/// </summary>
public class UpdatePromptCommandHandlerTests
{
    private readonly Mock<IPromptRepository> _mockRepository;
    private readonly Mock<IPromptMapper> _mockMapper;
    private readonly UpdatePromptCommandHandler _handler;

    public UpdatePromptCommandHandlerTests()
    {
        _mockRepository = new Mock<IPromptRepository>();
        _mockMapper = new Mock<IPromptMapper>();
        _handler = new UpdatePromptCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithExistingPrompt_ReturnsUpdatedPrompt()
    {
        // Arrange
        var promptId = Guid.NewGuid();
        var model = new UpdatePromptModel
        {
            Id = promptId,
            Title = "Updated Title",
            Body = "Updated Body"
        };

        var existingPrompt = new Prompt
        {
            Id = promptId,
            Title = "Old Title",
            Body = "Old Body"
        };

        var mappedEntity = new Prompt
        {
            Id = promptId,
            Title = model.Title,
            Body = model.Body
        };

        var updatedPrompt = new Prompt
        {
            Id = promptId,
            Title = model.Title,
            Body = model.Body
        };

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(promptId))
            .ReturnsAsync(existingPrompt);

        _mockMapper
            .Setup(mapper => mapper.ToEntity(model))
            .Returns(mappedEntity);

        _mockRepository
            .Setup(repo => repo.UpdateAsync(mappedEntity))
            .ReturnsAsync(updatedPrompt);

        var command = new UpdatePromptCommand(model);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(updatedPrompt);
        result!.Id.Should().Be(promptId);
        result.Title.Should().Be(model.Title);
        result.Body.Should().Be(model.Body);
        _mockRepository.Verify(repo => repo.GetByIdAsync(promptId), Times.Once);
        _mockMapper.Verify(mapper => mapper.ToEntity(model), Times.Once);
        _mockRepository.Verify(repo => repo.UpdateAsync(mappedEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentPrompt_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var model = new UpdatePromptModel
        {
            Id = nonExistentId,
            Title = "Title",
            Body = "Body"
        };

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Prompt?)null);

        var command = new UpdatePromptCommand(model);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(repo => repo.GetByIdAsync(nonExistentId), Times.Once);
        _mockMapper.Verify(mapper => mapper.ToEntity(It.IsAny<UpdatePromptModel>()), Times.Never);
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Prompt>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ExecutesSuccessfully()
    {
        // Arrange
        var promptId = Guid.NewGuid();
        var model = new UpdatePromptModel
        {
            Id = promptId,
            Title = "Test",
            Body = "Body"
        };

        var existingPrompt = new Prompt
        {
            Id = promptId,
            Title = "Old",
            Body = "Old"
        };

        var mappedEntity = new Prompt
        {
            Id = promptId,
            Title = model.Title,
            Body = model.Body
        };

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(promptId))
            .ReturnsAsync(existingPrompt);

        _mockMapper
            .Setup(mapper => mapper.ToEntity(model))
            .Returns(mappedEntity);

        _mockRepository
            .Setup(repo => repo.UpdateAsync(mappedEntity))
            .ReturnsAsync(mappedEntity);

        var command = new UpdatePromptCommand(model);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(mappedEntity);
    }
}
