using Domain.Db;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces.Infrastructure;

public interface IBrowsAiDbContext
{
    DbSet<Prompt> Prompts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

