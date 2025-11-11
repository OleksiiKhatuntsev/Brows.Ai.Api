using Domain.Interfaces.Infrastructure;

namespace Infrastructure.ExternalServices;

public class GeminiChatAgentFactory(string apiKey) : IGeminiChatAgentFactory
{
    private readonly string _apiKey = apiKey;

    public IGeminiChatAgent CreateAgent(string systemMessage)
    {
        return new GeminiChatAgent(_apiKey, systemMessage);
    }
}

