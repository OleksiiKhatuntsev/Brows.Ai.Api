using Domain.Db;

namespace Domain.Interfaces.Infrastructure;

public interface IPromptRepository
{
    Task<IEnumerable<Prompt>> GetAllAsync();
    Task<Prompt?> GetByIdAsync(Guid id);
    Task<Prompt> AddAsync(Prompt prompt);
}

