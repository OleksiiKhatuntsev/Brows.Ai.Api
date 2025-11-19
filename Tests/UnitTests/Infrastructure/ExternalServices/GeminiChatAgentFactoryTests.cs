using Domain.Interfaces.Infrastructure;
using FluentAssertions;
using Infrastructure.ExternalServices;
using Xunit;

namespace UnitTests.Infrastructure.ExternalServices;

/// <summary>
/// Unit tests for GeminiChatAgentFactory
/// </summary>
public class GeminiChatAgentFactoryTests
{
    private const string TestApiKey = "test-api-key-12345";
    private const string TestSystemMessage = "You are a helpful assistant.";

    [Fact]
    public void CreateAgent_WithValidInput_ReturnsNewAgent()
    {
        // Arrange
        var factory = new GeminiChatAgentFactory(TestApiKey);

        // Act
        var agent = factory.CreateAgent(TestSystemMessage);

        // Assert
        agent.Should().NotBeNull();
        agent.Should().BeAssignableTo<IGeminiChatAgent>();
    }

    [Fact]
    public void CreateAgent_CalledMultipleTimes_ReturnsUniqueInstances()
    {
        // Arrange
        var factory = new GeminiChatAgentFactory(TestApiKey);

        // Act
        var agent1 = factory.CreateAgent(TestSystemMessage);
        var agent2 = factory.CreateAgent(TestSystemMessage);

        // Assert
        agent1.Should().NotBeNull();
        agent2.Should().NotBeNull();
        agent1.Should().NotBeSameAs(agent2);
    }

    [Fact]
    public void CreateAgent_WithDifferentSystemMessages_ReturnsUniqueAgents()
    {
        // Arrange
        var factory = new GeminiChatAgentFactory(TestApiKey);
        var systemMessage1 = "You are a helpful assistant.";
        var systemMessage2 = "You are a coding assistant.";

        // Act
        var agent1 = factory.CreateAgent(systemMessage1);
        var agent2 = factory.CreateAgent(systemMessage2);

        // Assert
        agent1.Should().NotBeNull();
        agent2.Should().NotBeNull();
        agent1.Should().NotBe(agent2);
    }

    [Fact]
    public void CreateAgent_WithEmptySystemMessage_ReturnsAgent()
    {
        // Arrange
        var factory = new GeminiChatAgentFactory(TestApiKey);

        // Act
        var agent = factory.CreateAgent(string.Empty);

        // Assert
        agent.Should().NotBeNull();
        agent.Should().BeAssignableTo<IGeminiChatAgent>();
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_CreatesFactory()
    {
        // Arrange & Act
        var factory = new GeminiChatAgentFactory(string.Empty);

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeAssignableTo<IGeminiChatAgentFactory>();
    }
}
