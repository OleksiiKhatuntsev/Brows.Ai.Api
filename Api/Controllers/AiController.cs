using AutoGen.Core;
using Domain.Db;
using Domain.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Brows.Ai.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AiController(
    ILogger<AiController> logger,
    IGeminiWebService geminiWebService,
    IPromptRepository promptRepository)
    : ControllerBase
{
    private readonly ILogger<AiController> _logger = logger;

    [HttpGet(Name = "GetJoke")]
    public async Task<string> Get()
    {
        var response = await geminiWebService.SendRequest(
            "tell me a joke",
            "You are a helpful assistant."
        );
        return response.GetContent();
    }

    [HttpPost]
    public async Task<string> GetCustomResponse([FromBody] Prompt prompt)
    {
        var response = await geminiWebService.SendRequest(
            prompt.Body,
            "You are a helpful assistant."
        );
        return response.GetContent();
    }

    [HttpGet(Name = "GetAllPrompts")]
    public async Task<ActionResult<IEnumerable<Prompt>>> GetAllPrompts()
    {
        var prompts = await promptRepository.GetAllAsync();
        return Ok(prompts);
    }
}
