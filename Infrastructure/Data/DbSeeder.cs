using Domain.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BrowsAiDbContext>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if we already have data
        if (await context.Prompts.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed initial prompts
        var prompts = new List<Prompt>
        {
            new() {
                Id = Guid.NewGuid(),
                Title = "5 ideas for story",
                Body = "Give me 5 ideas for instagram story. I'm a brow master in Amsterdam"
            },
            new() {
                Id = Guid.NewGuid(),
                Title = "5 ideas for post",
                Body = "Give me 5 ideas for instagram post. I'm a brow master in Amsterdam"
            },
            new() {
                Id = Guid.NewGuid(),
                Title = "Plan next week",
                Body = "I'm a brow master, who need to create content plan for the next week. 7 stories, 1 post, 1 reel. My non-working days are Mon, Tue, Fri, Sun - this days I can do some content"
            },
            new() {
                Id = Guid.NewGuid(),
                Title = "Random SMM insight",
                Body = "Give me a random SMM insight. I'm a brow master"
            },
            new() {
                Id = Guid.NewGuid(),
                Title = "Random tip for AI video",
                Body = "Give me a random tip about \"How to create video using AI\""
            }
        };

        await context.Prompts.AddRangeAsync(prompts);
        await context.SaveChangesAsync();
    }
}

