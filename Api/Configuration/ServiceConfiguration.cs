using Domain.Configuration;
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

        services.AddScoped<IGeminiWebService>(sp =>
                {
                    var settings = sp
                        .GetRequiredService<IOptions<GeminiSettings>>()
                        .Value;
                    return new GeminiWebService(settings.ApiKey);
                }
            );

        // Register DbContext with SQLite
        services.AddDbContext<BrowsAiDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Register Repository
        services.AddScoped<IPromptRepository, PromptRepository>();
    }
}
