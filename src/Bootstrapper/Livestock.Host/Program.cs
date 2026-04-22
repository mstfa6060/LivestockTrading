using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Files.Features;
using Iam.Features;
using Livestock.Features;
using Shared.Abstractions.Identity;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Identity;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Swagger;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

// NSwag CLI boots the app to discover endpoint metadata; in that mode no
// real broker/cache is reachable, so swap heavyweight infra registrations
// for in-memory / no-op stubs. Triggered by `NSWAG_GENERATE=true`.
var isCodegen = string.Equals(
    Environment.GetEnvironmentVariable("NSWAG_GENERATE"), "true",
    StringComparison.OrdinalIgnoreCase);

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
if (isCodegen)
{
    builder.Services.AddFusionCache(); // in-memory only
}
else
{
    builder.Services.AddSharedCache(builder.Configuration);
}

// ── Messaging (NATS JetStream) ────────────────────────────────────────────────
if (isCodegen)
{
    builder.Services.AddSingleton<IEventPublisher, NoopEventPublisher>();
}
else
{
    builder.Services.AddSharedNats(builder.Configuration);
}

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
        s.OperationProcessors.Add(new RouteBasedOperationIdProcessor());
    };
    // /livestocktrading/Sellers/Nearby → tag = "Sellers" → AuthClient/SellersClient/etc.
    o.AutoTagPathSegmentIndex = 2;
    o.ShortSchemaNames = true;
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
