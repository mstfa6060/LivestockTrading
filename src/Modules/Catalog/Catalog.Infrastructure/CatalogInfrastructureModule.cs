using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Infrastructure.Caching;
using LivestockTrading.Catalog.Infrastructure.Persistence;
using LivestockTrading.Catalog.Infrastructure.RateProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shared.Contracts.Catalog;
using Shared.Contracts.Catalog.Admin;
using StackExchange.Redis;

namespace LivestockTrading.Catalog.Infrastructure;

/// <summary>
/// DI extension for Catalog.Infrastructure.
/// W3.0 placeholder — wire-up lands across later sub-batches:
/// repositories + IUnitOfWork (W3.3, DI host-wire W3.7), domain-event dispatch
/// interceptor (W3.4), cache foundation (W3.5A, this), read service + AdminRead (W3.5B),
/// rate providers + Quartz (W3.6), DbContext + interceptor wire + endpoint mapping
/// (W3.7, host-inert SON).
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

        // W3.5A: cache foundation — Plan-1 Karar 4 config-driven Catalog:CacheProvider
        // (default "Memory" — dev/test güvenli; "Redis" prod opt-in). Singleton tüm cache
        // services: Memory shared in-process state, Redis IConnectionMultiplexer connection
        // reuse pattern (StackExchange.Redis resmi, thread-safe internal pool).
        var cacheProvider = configuration["Catalog:CacheProvider"] ?? "Memory";
        if (string.Equals(cacheProvider, "Redis", StringComparison.OrdinalIgnoreCase))
        {
            var redisConn = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException(
                    "Redis CacheProvider seçildi ama ConnectionStrings:Redis yapılandırması eksik.");
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        // W3.5B.4: read services — ICatalogReadService (22 metot, W3.5B.2 impl) +
        // IAdminCatalogReadService (3 metot, W3.5B.3 impl). Lifetime Scoped: DbContext
        // Scoped (EF Core default) + ReadService DbContext dependency → Scoped zorunlu
        // (Singleton-in-Scoped capture memory leak + thread-safety).
        services.AddScoped<ICatalogReadService, CatalogReadService>();
        services.AddScoped<IAdminCatalogReadService, AdminCatalogReadService>();

        // W3.6.A: Cache decorator wrap — Scrutor Decorate pattern lifetime-preserve
        // (Scoped inner → Scoped outer). Negative caching YOK; 22 metot mapping CacheTtl
        // class'inda doc 05-catalog.md:604-618 grounded.
        services.Decorate<ICatalogReadService, CachedCatalogReadService>();

        // W3.6.B: Currency rate providers 3-tier (plan-doc 05-catalog.md §6:629-730).
        // TCMB (Tier 1, primary) → ECB (Tier 2, fallback) → Fawazahmed0 currency-api (Tier 3,
        // last-resort; W3.6.B Frontend kararı exchangerate.host swap, B.4 SENARYO-2 Aile 2).
        // Typed HttpClient (mock-friendly testability) + AddStandardResilienceHandler
        // (retry + timeout + circuit-breaker + rate-limiter Microsoft.Extensions.Http.Resilience).
        // 3 impl IRateProvider port'a kayitli → DI IEnumerable<IRateProvider> resolve eder
        // (W3.6.D Handler chain iteration icin); ayrica concrete tip resolve (typed client constraint).
        services.AddHttpClient<TcmbRateProvider>(client =>
        {
            client.BaseAddress = new Uri("https://www.tcmb.gov.tr/");
        })
        .AddStandardResilienceHandler();

        services.AddHttpClient<EcbRateProvider>(client =>
        {
            client.BaseAddress = new Uri("https://www.ecb.europa.eu/");
        })
        .AddStandardResilienceHandler();

        services.AddHttpClient<CurrencyApiRateProvider>(client =>
        {
            client.BaseAddress = new Uri("https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/");
        })
        .AddStandardResilienceHandler();

        // IRateProvider port → 3 concrete kayıt (W3.6.D Handler IEnumerable<IRateProvider> ile chain'i
        // tier sırasıyla iterate eder). AddHttpClient<TImpl> sadece concrete kaydeder; port kaydı manuel.
        services.AddTransient<IRateProvider>(sp => sp.GetRequiredService<TcmbRateProvider>());
        services.AddTransient<IRateProvider>(sp => sp.GetRequiredService<EcbRateProvider>());
        services.AddTransient<IRateProvider>(sp => sp.GetRequiredService<CurrencyApiRateProvider>());

        // Sıradaki: W3.6.C (Quartz scheduler + CurrencyRateUpdateJob), W3.6.D (event handlers +
        // CurrencyRateRefresher impl + RefreshExchangeRatesHandler swap), W3.7 (DbContext +
        // interceptor wire + endpoint mapping, host-inert SON).
        return services;
    }
}
