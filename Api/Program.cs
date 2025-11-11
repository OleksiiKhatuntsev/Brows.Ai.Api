using Brows.Ai.Api.Configuration;
using Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ServiceConfiguration.ConfigureAppSettings(builder.Services, builder.Configuration);
ServiceConfiguration.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BrowsAiDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DbSeeder.SeedAsync(context);
}

app.ConfigurePipeline(builder.Configuration);

app.Run();

// Make the implicit Program class public so it can be used by tests
public partial class Program { }

