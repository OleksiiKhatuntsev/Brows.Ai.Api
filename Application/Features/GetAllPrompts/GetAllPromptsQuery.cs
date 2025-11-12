using Domain.Db;
using MediatR;

namespace Application.Features.GetAllPrompts;

public record GetAllPromptsQuery : IRequest<IEnumerable<Prompt>>;

