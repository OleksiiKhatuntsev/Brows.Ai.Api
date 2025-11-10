using Domain.Db;
using Domain.Interfaces.Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PromptRepository(BrowsAiDbContext context) : IPromptRepository
{
    private readonly BrowsAiDbContext _context = context;

    public async Task<IEnumerable<Prompt>> GetAllAsync()
    {
        return await _context.Prompts.ToListAsync();
    }

    public async Task<Prompt?> GetByIdAsync(Guid id)
    {
        return await _context.Prompts.FindAsync(id);
    }

    public async Task<Prompt> AddAsync(Prompt prompt)
    {
        await _context.Prompts.AddAsync(prompt);
        await _context.SaveChangesAsync();
        return prompt;
    }
}

