using Microsoft.AspNetCore.Mvc;

namespace Brows.Ai.Api.Controllers
{
    using AutoGen.Core;
    using Domain.Interfaces.Infrastructure;

    [ApiController]
    [Route("[controller]/[action]")]
    public class AiController(
        ILogger<AiController> logger,
        IGeminiWebService geminiWebService)
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
    }
}
