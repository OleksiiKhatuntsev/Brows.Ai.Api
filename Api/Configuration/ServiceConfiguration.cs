namespace Brows.Ai.Api.Configuration;

using Domain.Configuration;
using Domain.Interfaces.Infrastructure;
using Infrastructure.ExternalServices;
using Microsoft.Extensions.Options;

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

    }
}
