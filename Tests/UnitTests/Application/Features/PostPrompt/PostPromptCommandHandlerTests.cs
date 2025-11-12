using Application.Features.PostPrompt;
using Domain.Db;
using Domain.Interfaces.Application;
using Domain.Interfaces.Infrastructure;
using Domain.Models.Api;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests.Application.Features.PostPrompt;

/// <summary>
/// Unit tests for PostPromptCommandHandler
/// </summary>
public class PostPromptCommandHandlerTests
{
    private readonly Mock<IPromptRepository> _mockRepository;
    private readonly Mock<IPromptMapper> _mockMapper;
    private readonly PostPromptCommandHandler _handler;

    public PostPromptCommandHandlerTests()
    {
        _mockRepository = new Mock<IPromptRepository>();
        _mockMapper = new Mock<IPromptMapper>();
        _handler = new PostPromptCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithValidModel_ShouldCreatePromptSuccessfully()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "Test Prompt",
            Body = "Test Body"
        };

        var mappedEntity = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Body = model.Body
        };

        var savedPrompt = new Prompt
        {
            Id = mappedEntity.Id,
            Title = mappedEntity.Title,
            Body = mappedEntity.Body
        };

        _mockMapper
            .Setup(mapper => mapper.ToEntity(model))
            .Returns(mappedEntity);

        _mockRepository
            .Setup(repo => repo.AddAsync(mappedEntity))
            .ReturnsAsync(savedPrompt);

        var command = new PostPromptCommand(model);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(savedPrompt);
        result.Id.Should().Be(mappedEntity.Id);
        result.Title.Should().Be(model.Title);
        result.Body.Should().Be(model.Body);
    }

    [Fact]
    public async Task Handle_ShouldCallMapperToEntityWithCorrectModel()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "Test Title",
            Body = "Test Body"
        };

        var mappedEntity = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Body = model.Body
        };

        _mockMapper
            .Setup(mapper => mapper.ToEntity(model))
            .Returns(mappedEntity);

        _mockRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Prompt>()))
            .ReturnsAsync(mappedEntity);

        var command = new PostPromptCommand(model);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockMapper.Verify(mapper => mapper.ToEntity(model), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryAddAsyncWithMappedEntity()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "New Prompt",
            Body = "New Body"
        };

        var mappedEntity = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Body = model.Body
        };

        _mockMapper
            .Setup(mapper => mapper.ToEntity(model))
            .Returns(mappedEntity);

        _mockRepository
            .Setup(repo => repo.AddAsync(mappedEntity))
            .ReturnsAsync(mappedEntity);

        var command = new PostPromptCommand(model);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(repo => repo.AddAsync(mappedEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSavedPromptFromRepository()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "Test",
            Body = "Body"
        };

        var mappedEntity = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Body = model.Body
        };

        var expectedSavedPrompt = new Prompt
        {
            Id = mappedEntity.Id,
            Title = mappedEntity.Title,
            Body = mappedEntity.Body
        };

        _mockMapper
            .Setup(mapper => mapper.ToEntity(model))
            .Returns(mappedEntity);

        _mockRepository
            .Setup(repo => repo.AddAsync(mappedEntity))
            .ReturnsAsync(expectedSavedPrompt);

        var command = new PostPromptCommand(model);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(expectedSavedPrompt);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldExecuteSuccessfully()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "Test Prompt",
            Body = "Test Body"
        };

        var mappedEntity = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Body = model.Body
        };

        _mockMapper
            .Setup(mapper => mapper.ToEntity(model))
            .Returns(mappedEntity);

        _mockRepository
            .Setup(repo => repo.AddAsync(mappedEntity))
            .ReturnsAsync(mappedEntity);

        var command = new PostPromptCommand(model);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(mappedEntity);
    }
}

