using AutoGen.Core;

namespace Domain.Interfaces.Infrastructure;

public interface IGeminiChatAgent
{
    Task<IMessage> SendAsync(string message);
}

