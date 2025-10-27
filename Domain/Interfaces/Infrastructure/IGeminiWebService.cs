namespace Domain.Interfaces.Infrastructure;

using AutoGen.Core;

public interface IGeminiWebService
{
    Task<IMessage> SendRequest(string prompt, string systemMessage);
}
