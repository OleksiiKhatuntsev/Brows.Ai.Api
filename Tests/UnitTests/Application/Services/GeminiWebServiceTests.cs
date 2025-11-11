using Application.Services;
using AutoGen.Core;
using Domain.Interfaces.Infrastructure;
using FluentAssertions;
using Moq;
using UnitTests.Fakes;
using Xunit;

namespace UnitTests.Application.Services;

/// <summary>
/// Unit tests for GeminiWebService
/// </summary>
public class GeminiWebServiceTests
{
    private readonly Mock<IGeminiChatAgentFactory> _mockFactory;
    private readonly FakeGeminiChatAgent _fakeAgent;
    private readonly GeminiWebService _service;

    public GeminiWebServiceTests()
    {
        _mockFactory = new Mock<IGeminiChatAgentFactory>();
        _fakeAgent = new FakeGeminiChatAgent();

        // Setup mock factory to return fake agent
        _mockFactory
            .Setup(f => f.CreateAgent(It.IsAny<string>()))
            .Returns(_fakeAgent);

        _service = new GeminiWebService(_mockFactory.Object);
    }

    [Fact]
    public async Task SendRequest_WithValidPromptAndSystemMessage_ShouldReturnResponse()
    {
        // Arrange
        var prompt = "Tell me a joke";
        var systemMessage = "You are a helpful assistant.";

        // Act
        var result = await _service.SendRequest(prompt, systemMessage);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IMessage>();
    }

    [Fact]
    public async Task SendRequest_ShouldUseFactoryToCreateAgent()
    {
        // Arrange
        var prompt = "Test prompt";
        var systemMessage = "Test system message";

        // Act
        await _service.SendRequest(prompt, systemMessage);

        // Assert
        _mockFactory.Verify(f => f.CreateAgent(systemMessage), Times.Once);
    }

    [Fact]
    public async Task SendRequest_WithDifferentSystemMessages_ShouldCallFactoryEachTime()
    {
        // Arrange
        var prompt = "Test prompt";
        var systemMessage1 = "You are a helpful assistant.";
        var systemMessage2 = "You are a coding assistant.";

        // Act
        await _service.SendRequest(prompt, systemMessage1);
        await _service.SendRequest(prompt, systemMessage2);

        // Assert
        _mockFactory.Verify(f => f.CreateAgent(systemMessage1), Times.Once);
        _mockFactory.Verify(f => f.CreateAgent(systemMessage2), Times.Once);
    }

    [Fact]
    public async Task SendRequest_ShouldReturnAgentResponse()
    {
        // Arrange
        var prompt = "Hello";
        var systemMessage = "You are a helpful assistant.";

        // Act
        var result = await _service.SendRequest(prompt, systemMessage);

        // Assert
        result.Should().NotBeNull();
        result.GetContent().Should().Contain("Fake response to: Hello");
    }

    [Fact]
    public async Task SendRequest_WithEmptyPrompt_ShouldStillWork()
    {
        // Arrange
        var prompt = string.Empty;
        var systemMessage = "You are a helpful assistant.";

        // Act
        var result = await _service.SendRequest(prompt, systemMessage);

        // Assert
        result.Should().NotBeNull();
        _mockFactory.Verify(f => f.CreateAgent(systemMessage), Times.Once);
    }

    [Fact]
    public async Task SendRequest_WithEmptySystemMessage_ShouldStillWork()
    {
        // Arrange
        var prompt = "Test prompt";
        var systemMessage = string.Empty;

        // Act
        var result = await _service.SendRequest(prompt, systemMessage);

        // Assert
        result.Should().NotBeNull();
        _mockFactory.Verify(f => f.CreateAgent(string.Empty), Times.Once);
    }
}

