using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Arfware.ArfBlocks.Core.Extentions;
using Arfware.ArfBlocks.Core;
using BaseModules.IAM.Application.Configuration;
using Common.Services.Caching.Extensions;
using Common.Services.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Logging
builder.AddSerilogLogging("IAM.Api");

string DefaultCorsPolicy = "DefaultCorsPolicy";

// CORS ayarları
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DefaultCorsPolicy,
        policyBuilder =>
        {
            policyBuilder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

// Add Maden Caching (includes MemoryCache, DistributedCache, and Redis ConnectionMultiplexer)
builder.Services.AddMadenCaching(builder.Configuration);

// ArfBlocks ayarları
builder.Services.AddArfBlocks(options =>
{
    options.ApplicationProjectNamespace = "BaseModules.IAM.Application";
    options.ConfigurationSection = builder.Configuration.GetSection("ProjectConfigurations");
    options.LogLevel = LogLevels.Warning;
    // options.PreOperateHandler = typeof(BaseModules.IAM.Application.DefaultHandlers.Operators.Commands.PreOperate.Handler);
    // options.PostOperateHandler = typeof(BaseModules.IAM.Application.DefaultHandlers.Operators.Commands.PostOperate.Handler);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Correlation ID Middleware
app.UseCorrelationId();

// Serilog Request Logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
});

// CORS kullanımı
app.UseCors(DefaultCorsPolicy);

// ArfBlocks middleware - EN SONDA KALMALI
app.UseArfBlocksRequestHandlers(options =>
{
    // options.AuthorizationOptions.Audience = JwtService.Audience;
    // options.AuthorizationOptions.Secret = JwtService.Secret;
});

Log.Information("IAM API Started!");
Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "IAM API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
