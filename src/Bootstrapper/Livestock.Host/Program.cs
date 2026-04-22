using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Files.Features;
using Iam.Features;
using Livestock.Features;
using Shared.Abstractions.Identity;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ───────────────────────────────────────────────────────────────────
builder.AddSharedSerilog("livestock-host");

// ── Observability ─────────────────────────────────────────────────────────────
builder.Services.AddSharedOpenTelemetry(builder.Configuration, "livestock-host");

// ── Auth ──────────────────────────────────────────────────────────────────────
builder.Services
    .AddAuthenticationJwtBearer(
        s =>
        {
            s.SigningKey = builder.Configuration["Jwt:SigningKey"]
                ?? throw new InvalidOperationException("Jwt:SigningKey is required.");
        },
        b => b.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            // SignalR WebSocket cannot send Authorization header; read JWT from query string
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
        })
    .AddAuthorization();

// ── Cache (FusionCache L1+L2) ─────────────────────────────────────────────────
builder.Services.AddSharedCache(builder.Configuration);

// ── Messaging (NATS JetStream) ────────────────────────────────────────────────
builder.Services.AddSharedNats(builder.Configuration);

// ── HTTP Context / User Context ───────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpUserContext>();

// ── SignalR ───────────────────────────────────────────────────────────────────
builder.Services.AddSignalR();

// ── FastEndpoints ─────────────────────────────────────────────────────────────
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "GlobalLivestock API";
        s.Version = "v1";
    };
});

// ── Modules ───────────────────────────────────────────────────────────────────
builder.Services.AddIamModule(builder.Configuration);
builder.Services.AddFilesModule(builder.Configuration);
builder.Services.AddLivestockModule(builder.Configuration);

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.MapHealthChecks("/health");
app.MapHub<Livestock.Features.Hubs.ChatHub>("/hubs/chat");

app.Run();
