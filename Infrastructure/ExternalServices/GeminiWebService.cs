namespace Infrastructure.ExternalServices;

using AutoGen.Core;
using AutoGen.Gemini;
using Domain.Interfaces.Infrastructure;

public class GeminiWebService(string apiKey) : IGeminiWebService
{
    public async Task<IMessage> SendRequest(string prompt, string systemMessage)
    {
        if (apiKey == string.Empty)
        {
            Console.WriteLine("Set GOOGLE_GEMINI_API_KEY environment variable.");
            return null;
        }

        var geminiAgent = new GeminiChatAgent(
                name: "gemini",
                model: "gemini-2.5-flash",
                apiKey: apiKey,
                systemMessage: systemMessage)
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var reply = await geminiAgent.SendAsync(prompt);
        return reply;
    }
}
