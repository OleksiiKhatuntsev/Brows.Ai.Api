using Domain.Db;
using Domain.Models.Api;

namespace Domain.Interfaces.Application;

public interface IPromptMapper
{
    Prompt ToEntity(PostPromptModel model);
    Prompt ToEntity(UpdatePromptModel model);
}

