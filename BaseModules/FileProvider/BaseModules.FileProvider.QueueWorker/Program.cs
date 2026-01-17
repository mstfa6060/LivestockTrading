using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.FileProviders;
using BaseModules.FileProvider.QueueWorker.Workers;
using BaseModules.FileProvider.QueueWorker.Models;
using Arfware.ArfBlocks.Core.Extentions;
using Arfware.ArfBlocks.Core;

var builder = WebApplication.CreateBuilder(args);

// Worker as Background Service
builder.Services.Configure<FileCreateQueueKafkaOptions>(builder.Configuration.GetSection("FileCreateQueueKafkaOptions"));
builder.Services.Configure<FileApproveQueueKafkaOptions>(builder.Configuration.GetSection("FileApproveQueueKafkaOptions"));
builder.Services.AddHostedService<FileCreateQueueWorker>();
builder.Services.AddHostedService<FileApproveQueueWorker>();


string DefaultCorsPolicy = "DefaultCorsPolicy";
builder.Services.AddCors(options =>
{
    // Development Cors Policy
    options.AddPolicy(name: DefaultCorsPolicy,
        builder =>
        {
            builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        });
});

// ArfBlocks Dependencies
builder.Services.AddArfBlocks(options =>
{
    options.ApplicationProjectNamespace = "BaseModules.FileProvider.Application";
    options.ConfigurationSection = builder.Configuration.GetSection("ProjectConfigurations");
    options.LogLevel = LogLevels.Warning;
});

var app = builder.Build();
var env = builder.Environment;

var storePath = Path.Combine(env.ContentRootPath, "wwwroot");
if (!Directory.Exists(storePath))
    Directory.CreateDirectory(storePath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storePath),
});

app.UseCors(DefaultCorsPolicy);

app.UseArfBlocksRequestHandlers(options =>
{
    // options.AuthorizationOptions.Audience = JwtService.Audience;
    // options.AuthorizationOptions.Secret = JwtService.Secret;
});

app.Run();
