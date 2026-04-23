using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace Shared.Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSharedSerilog(this WebApplicationBuilder builder, string serviceName)
    {
        builder.Host.UseSerilog((ctx, cfg) =>
        {
            cfg.ReadFrom.Configuration(ctx.Configuration)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithProperty("ServiceName", serviceName)
               .WriteTo.Console(LogEventLevel.Information)
               .WriteTo.Seq(
                   ctx.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341",
                   LogEventLevel.Information)
               .WriteTo.OpenTelemetry(opts =>
               {
                   opts.Endpoint = ctx.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317";
                   opts.ResourceAttributes.Add("service.name", serviceName);
               });
        });

        return builder;
    }
}
