using Domain.Db;
using Domain.Models.Api;
using MediatR;

namespace Application.Features.UpdatePrompt;

public record UpdatePromptCommand(UpdatePromptModel Model) : IRequest<Prompt?>;

