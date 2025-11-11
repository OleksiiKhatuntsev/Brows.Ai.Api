using AutoGen.Core;
using AutoGen.Gemini;
using Domain.Interfaces.Infrastructure;

namespace Infrastructure.ExternalServices;

public class GeminiChatAgent : IGeminiChatAgent
{
    private readonly IAgent _agent;

    public GeminiChatAgent(string apiKey, string systemMessage)
    {
        _agent = new AutoGen.Gemini.GeminiChatAgent(
                name: "gemini",
                model: "gemini-2.5-flash",
                apiKey: apiKey,
                systemMessage: systemMessage)
            .RegisterMessageConnector()
            .RegisterPrintMessage();
    }

    public async Task<IMessage> SendAsync(string message)
    {
        return await _agent.SendAsync(message);
    }
}

