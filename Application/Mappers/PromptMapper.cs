using Domain.Db;
using Domain.Interfaces.Application;
using Domain.Models.Api;

namespace Application.Mappers;

public class PromptMapper : IPromptMapper
{
    public Prompt ToEntity(PostPromptModel model)
    {
        return new Prompt
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Body = model.Body
        };
    }

    public Prompt ToEntity(UpdatePromptModel model)
    {
        return new Prompt
        {
            Id = model.Id,
            Title = model.Title,
            Body = model.Body
        };
    }
}

