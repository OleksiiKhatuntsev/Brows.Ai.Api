using Application.Services;
using Domain.Configuration;
using Domain.Interfaces.Application;
using Domain.Interfaces.Infrastructure;
using Infrastructure.Data;
using Infrastructure.ExternalServices;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Brows.Ai.Api.Configuration;

public static class ServiceConfiguration
{
    public static void ConfigureAppSettings(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GeminiSettings>(
            configuration.GetSection("GoogleApiSettings")
        );
    }

    public static void RegisterServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        services.AddOpenApi();
        services.AddSwaggerGen();

        // In your Program.cs or Startup.cs
        services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowReactApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            }
        );

        // Register Gemini Chat Agent Factory
        services.AddScoped<IGeminiChatAgentFactory>(sp =>
        {
            var settings = sp
                .GetRequiredService<IOptions<GeminiSettings>>()
                .Value;
            return new GeminiChatAgentFactory(settings.ApiKey);
        });

        // Register Gemini Web Service
        services.AddScoped<IGeminiWebService, GeminiWebService>();

        // Register DbContext with SQLite
        services.AddDbContext<BrowsAiDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Register IBrowsAiDbContext interface
        services.AddScoped<IBrowsAiDbContext>(provider =>
            provider.GetRequiredService<BrowsAiDbContext>());

        // Register Repository
        services.AddScoped<IPromptRepository, PromptRepository>();
    }
}
