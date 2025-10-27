namespace Brows.Ai.Api.Configuration;

using Domain.Configuration;
using Domain.Interfaces.Infrastructure;
using Infrastructure.ExternalServices;
using Microsoft.Extensions.Options;

public static class ServiceConfiguration
{
    public static void ConfigureAppSettings(IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GeminiSettings>(
            configuration.GetSection("GoogleApiSettings")
        );
    }

    public static void RegisterServices(IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddSwaggerGen();
        
        services.AddScoped<IGeminiWebService>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<GeminiSettings>>().Value;
            return new GeminiWebService(settings.ApiKey);
        });
    }
}
