namespace Domain.Interfaces.Infrastructure;

public interface IGeminiChatAgentFactory
{
    IGeminiChatAgent CreateAgent(string systemMessage);
}

