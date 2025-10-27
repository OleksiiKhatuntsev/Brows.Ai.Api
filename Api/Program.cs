using Brows.Ai.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ServiceConfiguration.ConfigureAppSettings(builder.Services, builder.Configuration);
ServiceConfiguration.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline(builder.Configuration);

app.Run();
