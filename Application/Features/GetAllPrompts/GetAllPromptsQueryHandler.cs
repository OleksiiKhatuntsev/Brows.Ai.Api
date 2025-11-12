using Domain.Db;
using Domain.Interfaces.Infrastructure;
using MediatR;

namespace Application.Features.GetAllPrompts;

public class GetAllPromptsQueryHandler(IPromptRepository promptRepository)
    : IRequestHandler<GetAllPromptsQuery, IEnumerable<Prompt>>
{
    public async Task<IEnumerable<Prompt>> Handle(
        GetAllPromptsQuery request,
        CancellationToken cancellationToken)
    {
        return await promptRepository.GetAllAsync();
    }
}

