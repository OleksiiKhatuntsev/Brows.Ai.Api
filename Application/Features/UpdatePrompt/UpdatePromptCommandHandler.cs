using Domain.Db;
using Domain.Interfaces.Application;
using Domain.Interfaces.Infrastructure;
using MediatR;

namespace Application.Features.UpdatePrompt;

public class UpdatePromptCommandHandler(
    IPromptRepository promptRepository,
    IPromptMapper promptMapper)
    : IRequestHandler<UpdatePromptCommand, Prompt?>
{
    public async Task<Prompt?> Handle(
        UpdatePromptCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await promptRepository.GetByIdAsync(request.Model.Id);
        if (existing == null)
        {
            return null;
        }

        var promptEntity = promptMapper.ToEntity(request.Model);

        var updatedPrompt = await promptRepository.UpdateAsync(promptEntity);

        return updatedPrompt;
    }
}

