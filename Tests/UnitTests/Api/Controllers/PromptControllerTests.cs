using Application.Features.GetAllPrompts;
using Application.Features.PostPrompt;
using Application.Features.UpdatePrompt;
using Brows.Ai.Api.Controllers;
using Domain.Db;
using Domain.Models.Api;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests.Api.Controllers;

/// <summary>
/// Unit tests for PromptController
/// </summary>
public class PromptControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly PromptController _controller;

    public PromptControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new PromptController(_mockMediator.Object);
    }

    #region PostPrompt Tests

    [Fact]
    public async Task PostPrompt_WithValidModel_ReturnsCreatedResult()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "Test Prompt",
            Body = "Test Body"
        };

        var savedPrompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Body = model.Body
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<PostPromptCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedPrompt);

        // Act
        var result = await _controller.PostPrompt(model);

        // Assert
        var actionResult = result.Result as CreatedAtActionResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(201);
        actionResult.ActionName.Should().Be(nameof(_controller.GetAllPrompts));
        actionResult.RouteValues.Should().ContainKey("id");
        actionResult.RouteValues!["id"].Should().Be(savedPrompt.Id);

        var returnedPrompt = actionResult.Value as Prompt;
        returnedPrompt.Should().NotBeNull();
        returnedPrompt.Should().BeSameAs(savedPrompt);
        returnedPrompt!.Id.Should().Be(savedPrompt.Id);
        returnedPrompt.Title.Should().Be(model.Title);
        returnedPrompt.Body.Should().Be(model.Body);
    }

    [Fact]
    public async Task PostPrompt_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var model = new PostPromptModel
        {
            Title = "Test",
            Body = "Body"
        };

        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.PostPrompt(model);

        // Assert
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(400);
        actionResult.Value.Should().BeOfType<SerializableError>();
    }

    #endregion

    #region UpdatePrompt Tests

    [Fact]
    public async Task UpdatePrompt_WithExistingPrompt_ReturnsOkResult()
    {
        // Arrange
        var promptId = Guid.NewGuid();
        var model = new UpdatePromptModel
        {
            Id = promptId,
            Title = "Updated Title",
            Body = "Updated Body"
        };

        var updatedPrompt = new Prompt
        {
            Id = promptId,
            Title = model.Title,
            Body = model.Body
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdatePromptCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedPrompt);

        // Act
        var result = await _controller.UpdatePrompt(model);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        var returnedPrompt = actionResult.Value as Prompt;
        returnedPrompt.Should().NotBeNull();
        returnedPrompt.Should().BeSameAs(updatedPrompt);
        returnedPrompt!.Id.Should().Be(promptId);
        returnedPrompt.Title.Should().Be(model.Title);
        returnedPrompt.Body.Should().Be(model.Body);
    }

    [Fact]
    public async Task UpdatePrompt_WithNonExistentPrompt_ReturnsNotFoundWithMessage()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var model = new UpdatePromptModel
        {
            Id = nonExistentId,
            Title = "Title",
            Body = "Body"
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdatePromptCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Prompt?)null);

        // Act
        var result = await _controller.UpdatePrompt(model);

        // Assert
        var actionResult = result.Result as NotFoundObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(404);

        var errorObject = actionResult.Value;
        errorObject.Should().NotBeNull();

        var message = errorObject!.GetType().GetProperty("message")?.GetValue(errorObject, null) as string;
        message.Should().Be($"Prompt with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task UpdatePrompt_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var model = new UpdatePromptModel
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Body = "Body"
        };

        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.UpdatePrompt(model);

        // Assert
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(400);
        actionResult.Value.Should().BeOfType<SerializableError>();
    }

    #endregion

    #region GetAllPrompts Tests

    [Fact]
    public async Task GetAllPrompts_WithExistingPrompts_ReturnsOkWithAllPrompts()
    {
        // Arrange
        var prompts = new List<Prompt>
        {
            new() { Id = Guid.NewGuid(), Title = "Prompt 1", Body = "Body 1" },
            new() { Id = Guid.NewGuid(), Title = "Prompt 2", Body = "Body 2" },
            new() { Id = Guid.NewGuid(), Title = "Prompt 3", Body = "Body 3" }
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllPromptsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prompts);

        // Act
        var result = await _controller.GetAllPrompts();

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        var returnedPrompts = actionResult.Value as IEnumerable<Prompt>;
        returnedPrompts.Should().NotBeNull();
        returnedPrompts.Should().HaveCount(3);
        returnedPrompts.Should().BeEquivalentTo(prompts);
    }

    [Fact]
    public async Task GetAllPrompts_WithNoPrompts_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyList = new List<Prompt>();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllPromptsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllPrompts();

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        var returnedPrompts = actionResult.Value as IEnumerable<Prompt>;
        returnedPrompts.Should().NotBeNull();
        returnedPrompts.Should().BeEmpty();
    }

    #endregion
}

