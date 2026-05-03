using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Arfware.ArfBlocks.Core.Extentions;
using Arfware.ArfBlocks.Core;
using Common.Services.Caching.Extensions;
using Common.Services.Logging;
using Common.Helpers;
using Serilog;
using LivestockTrading.Api.Converters;
using LivestockTrading.Api.Hubs;
using LivestockTrading.Api.SignalR;
using LivestockTrading.Application.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Services.Auth.JsonWebToken;

var builder = WebApplication.CreateBuilder(args);

// Serilog Logging
builder.AddSerilogLogging("LivestockTrading.Api");

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

// JWT Authentication (ChatHub [Authorize] icin gerekli — SignalR negotiate auth challenge ediyor)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtService.Secret)),
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidAudience = JwtService.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "nameid"
        };

        // SignalR JS client WebSocket'te Authorization header set edemez,
        // accessTokenFactory query string'e ekler (?access_token=...)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// SignalR for real-time messaging
var redisConnectionString = builder.Configuration["Caching:Redis:ConnectionString"];
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSignalR()
        .AddStackExchangeRedis(redisConnectionString, options =>
        {
            options.Configuration.ChannelPrefix = "LivestockTrading";
        });
}
else
{
    builder.Services.AddSignalR();
}

// SignalR broadcast abstraction (Application -> Api boundary)
builder.Services.AddScoped<IChatNotifier, SignalRChatNotifier>();

// ArfBlocks Dependencies
builder.Services.AddArfBlocks(options =>
{
    options.ApplicationProjectNamespace = "LivestockTrading.Application";
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

// Auth (UseCors'dan SONRA, MapHub'dan ONCE)
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// SignalR Hubs
app.MapHub<ChatHub>("/hubs/chat");

// ArfBlocks Request Handlers
app.UseArfBlocksRequestHandlers(options => { });

Log.Information("LivestockTrading API Started!");
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
