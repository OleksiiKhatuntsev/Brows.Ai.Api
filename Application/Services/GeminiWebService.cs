using AutoGen.Core;
using Domain.Interfaces.Application;
using Domain.Interfaces.Infrastructure;

namespace Application.Services;

public class GeminiWebService : IGeminiWebService
{
    private readonly IGeminiChatAgentFactory _agentFactory;

    public GeminiWebService(IGeminiChatAgentFactory agentFactory)
    {
        _agentFactory = agentFactory;
    }

    public async Task<IMessage> SendRequest(string prompt, string systemMessage)
    {
        var agent = _agentFactory.CreateAgent(systemMessage);
        var reply = await agent.SendAsync(prompt);
        return reply;
    }
}

