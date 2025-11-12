using Domain.Db;
using Domain.Interfaces.Application;
using Domain.Interfaces.Infrastructure;
using MediatR;

namespace Application.Features.PostPrompt;

public class PostPromptCommandHandler(
    IPromptRepository promptRepository,
    IPromptMapper promptMapper)
    : IRequestHandler<PostPromptCommand, Prompt>
{
    public async Task<Prompt> Handle(
        PostPromptCommand request,
        CancellationToken cancellationToken)
    {
        var promptEntity = promptMapper.ToEntity(request.Model);

        var savedPrompt = await promptRepository.AddAsync(promptEntity);

        return savedPrompt;
    }
}

