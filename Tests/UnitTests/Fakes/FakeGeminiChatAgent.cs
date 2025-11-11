using AutoGen.Core;
using Domain.Interfaces.Infrastructure;

namespace UnitTests.Fakes;

public class FakeGeminiChatAgent : IGeminiChatAgent
{
    public async Task<IMessage> SendAsync(string message)
    {
        var response = new TextMessage(
            Role.Assistant,
            $"Fake response to: {message}",
            from: "fake-gemini"
        );

        return await Task.FromResult(response);
    }
}

