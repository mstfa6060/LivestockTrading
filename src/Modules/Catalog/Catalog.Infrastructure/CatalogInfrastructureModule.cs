using LivestockTrading.Catalog.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LivestockTrading.Catalog.Infrastructure;

/// <summary>
/// DI extension for Catalog.Infrastructure.
/// W3.0 placeholder — wire-up lands across later sub-batches:
/// repositories + IUnitOfWork (W3.3, DI host-wire W3.7), domain-event dispatch
/// interceptor (W3.4, this), read service + cache (W3.5), rate providers + Quartz (W3.6),
/// DbContext + interceptor wire + endpoint mapping (W3.7, host-inert SON).
/// Doc grounding: 01-architecture.md:185 (only LivestockTrading.Api host references
/// module .Infrastructure for DI registration); 03-domain-patterns.md Kural 5.
/// </summary>
public static class CatalogInfrastructureModule
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // W3.4: domain event dispatch interceptor (KARAR 4 (b) — DbContext registration
        // W3.7 host-wire'da; orada `opts.AddInterceptors(sp.GetRequiredService<
        // DomainEventDispatchInterceptor>())` ile wire edilecek. Karar 1.a "BuildMigrator
        // only Faz 1" + KAYDET-17 use-case driven gerekçesi: BuildApp varyantı W3.7'de
        // AddDbContext ile tek noktada gelir).
        services.AddScoped<DomainEventDispatchInterceptor>();

        // Sıradaki: W3.5 (read+cache), W3.6 (rate providers + Quartz),
        // W3.7 (DbContext + interceptor wire + endpoint mapping, host-inert SON).
        return services;
    }
}
