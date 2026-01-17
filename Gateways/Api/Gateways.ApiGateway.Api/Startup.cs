using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;

namespace Gateways.ApiGateway.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public readonly string DefaultCorsPolicyName = "DefaultCorsPolicy";
    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Authorization
        string secretKey = "Rj!7dXrQ*5z9@Lb^PqKf!M2&gUw#AeZx7Vp$ChN+36YT@tW%";
        services.AddAuthentication()
        .AddJwtBearer(secretKey, x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                // Token Lifetime Validation 
                ValidateLifetime = true,
                LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                 TokenValidationParameters validationParameters) =>
                {
                    return expires >= DateTime.UtcNow;
                }
            };
        });

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicyName,
                builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddControllers();
        services.AddOcelot(Configuration);
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "OcelotForUI", Version = "v1" });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OcelotForUI v1"));
        }


        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(async () =>
                {
                    int responseStatusCode = context.Response.StatusCode;
                    if (responseStatusCode == (int)HttpStatusCode.Unauthorized)
                    {
                        byte[] contentAsBytes = Encoding.ASCII.GetBytes("Unauthorized");
                        await context.Response.Body.WriteAsync(contentAsBytes, 0, contentAsBytes.Length);
                    }
                });
            await next();
        });

        app.UseCors(DefaultCorsPolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseWebSockets();
        await app.UseOcelot();
    }
}