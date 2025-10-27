namespace Brows.Ai.Api.Configuration;

public static class AppConfiguration
{
    public static WebApplication ConfigurePipeline(this WebApplication app, IConfiguration configuration)
    {
        if (app is not null)
        {
            if (app.Environment.IsDevelopment())
            {
                app.ConfigureDevelopmentEnvironment();
            }
            else
            {
                app.ConfigureProductionEnvironment();
            }

            app.ConfigureCommonEnvironment(configuration);
        }

        return app;
    }
    
    private static void ConfigureDevelopmentEnvironment(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    private static void ConfigureProductionEnvironment(this WebApplication app)
    {}
    
    private static void ConfigureCommonEnvironment(this WebApplication app, IConfiguration configuration)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}
