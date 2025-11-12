using Domain.Db;
using Domain.Models.Api;
using MediatR;

namespace Application.Features.PostPrompt;

public record PostPromptCommand(PostPromptModel Model) : IRequest<Prompt>;

