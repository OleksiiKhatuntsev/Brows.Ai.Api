using Brows.Ai.Api.Configuration;
using Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ServiceConfiguration.ConfigureAppSettings(builder.Services, builder.Configuration);
ServiceConfiguration.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Seed initial data
await DbSeeder.SeedDataAsync(app.Services);

app.ConfigurePipeline(builder.Configuration);

app.Run();
