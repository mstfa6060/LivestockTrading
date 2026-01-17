using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Arfware.ArfBlocks.Core.Extentions;
using Arfware.ArfBlocks.Core;
using Common.Services.Caching.Extensions;
using Common.Services.Logging;
using Common.Helpers;
using Serilog;
using GlobalLivestock.Api.Converters;

var builder = WebApplication.CreateBuilder(args);

// Serilog Logging
builder.AddSerilogLogging("GlobalLivestock.Api");

// CORS Policy
string DefaultCorsPolicy = "DefaultCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DefaultCorsPolicy,
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed(_ => true)
                   .AllowCredentials();
        });
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // DateTime'lari UTC formatinda 'Z' suffix ile serialize et
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

        // UTC DateTime icin custom converter
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeNullableConverter());
    });

// Maden Caching
builder.Services.AddMadenCaching(builder.Configuration);

// NotificationHelper
builder.Services.AddSingleton<NotificationHelper>();

// MemoryCache Configuration
builder.Services.AddMemoryCache(options =>
{
});

// ArfBlocks Dependencies
builder.Services.AddArfBlocks(options =>
{
    options.ApplicationProjectNamespace = "GlobalLivestock.Application";
    options.ConfigurationSection = builder.Configuration.GetSection("ProjectConfigurations");
    options.LogLevel = LogLevels.Warning;
});

var app = builder.Build();

// Correlation ID Middleware
app.UseCorrelationId();

// Serilog Request Logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
});

// CORS
app.UseCors(DefaultCorsPolicy);

// Controllers
app.MapControllers();

// ArfBlocks Request Handlers
app.UseArfBlocksRequestHandlers(options => { });

Log.Information("GlobalLivestock API Started!");
Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
