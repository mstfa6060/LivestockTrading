using Arfware.ArfBlocks.Core.Extentions;
using Arfware.ArfBlocks.Core;
using Microsoft.AspNetCore.StaticFiles;
using BaseModules.FileProvider.Application.Configuration;
using Common.Services.Environment;
using BaseModules.FileProvider.Api.Middlewares;
using Common.Services.Caching.Extensions;

var builder = WebApplication.CreateBuilder(args);

// var configurations = builder.Configuration.GetSection("ProjectConfigurations").Get<ProjectConfigurations>();
// var environmentService = new EnvironmentService(configurations.EnvironmentConfiguration);

// Caching Service (Redis)
builder.Services.AddMadenCaching(builder.Configuration);

builder.Services.AddControllers();

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

// MemoryCache Configuration - Fix for "Cache entry must specify a value for Size when SizeLimit is set" error
builder.Services.AddMemoryCache(options =>
{
	// Remove SizeLimit to avoid cache size errors
	// options.SizeLimit = null; // This is default behavior when not set
});

// ArfBlocks Dependencies
builder.Services.AddArfBlocks(options =>
{
	options.ApplicationProjectNamespace = "BaseModules.FileProvider.Application";
	options.ConfigurationSection = builder.Configuration.GetSection("ProjectConfigurations");
	options.LogLevel = LogLevels.Warning;
	options.PreOperateHandler = typeof(BaseModules.FileProvider.Application.DefaultHandlers.Operators.Commands.PreOperate.Handler);
	options.PostOperateHandler = typeof(BaseModules.FileProvider.Application.DefaultHandlers.Operators.Commands.PostOperate.Handler);
});


var app = builder.Build();

AccessControlMiddleware.UseAccessControl(app);

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".eyp"] = "application/eyazisma";
provider.Mappings[".msg"] = "application/vnd.ms-outlook";
provider.Mappings[".ppt"] = "application/vnd.ms-powerpoint";
provider.Mappings[".pptx"] = "application/vnd.openxmlformats";
provider.Mappings[".rar"] = "application/vnd.rar";
provider.Mappings[".doc"] = "application/msword";
provider.Mappings[".docx"] = "application/vnd.openxmlformats";
provider.Mappings[".html"] = "text/html";
provider.Mappings[".ico"] = "image/vnd.microsoft.icon";
provider.Mappings[".jpeg"] = "image/jpeg";
provider.Mappings[".jpg"] = "image/jpeg";
provider.Mappings[".otf"] = "font/otf";
provider.Mappings[".png"] = "image/png";
provider.Mappings[".svg"] = "image/svg+xml";
provider.Mappings[".xml"] = "application/xml";
provider.Mappings[".zip"] = "application/zip";
app.UseStaticFiles(new StaticFileOptions
{
	OnPrepareResponse = ctx =>
	{
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
		ctx.Context.Response.Headers.Append("Access-Control-Max-Age", "86400");
		ctx.Context.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition, Content-Length, Content-Type, Last-Modified, ETag, Accept-Ranges");
	},
	ContentTypeProvider = provider,
});

app.UseCors(DefaultCorsPolicy);

// app.UseArfBlocksRequestHandlers(options =>
// {
//     // options.AuthorizationOptions.Audience = JwtService.Audience;
//     // options.AuthorizationOptions.Secret = JwtService.Secret;
// });

// app.Run();

app.MapControllers();
app.Run();
