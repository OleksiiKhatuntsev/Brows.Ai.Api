using Application.Features.GetAllPrompts;
using Application.Features.PostPrompt;
using Application.Features.UpdatePrompt;
using Domain.Db;
using Domain.Models.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Brows.Ai.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PromptController(IMediator mediator)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Prompt>> PostPrompt([FromBody] PostPromptModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var savedPrompt = await mediator.Send(new PostPromptCommand(model));

        return CreatedAtAction(
            nameof(GetAllPrompts),
            new { id = savedPrompt.Id },
            savedPrompt);
    }

    [HttpPut]
    public async Task<ActionResult<Prompt>> UpdatePrompt([FromBody] UpdatePromptModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedPrompt = await mediator.Send(new UpdatePromptCommand(model));

        if (updatedPrompt == null)
        {
            return NotFound(new { message = $"Prompt with ID {model.Id} not found" });
        }

        return Ok(updatedPrompt);
    }

    [HttpGet(Name = "GetAllPrompts")]
    public async Task<ActionResult<IEnumerable<Prompt>>> GetAllPrompts()
    {
        var prompts = await mediator.Send(new GetAllPromptsQuery());
        return Ok(prompts);
    }
}
