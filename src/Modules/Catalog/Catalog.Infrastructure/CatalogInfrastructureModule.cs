using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LivestockTrading.Catalog.Infrastructure;

/// <summary>
/// DI extension for Catalog.Infrastructure.
/// W3.0 placeholder — no registrations yet. Wire-up lands across later sub-batches:
/// repositories + IUnitOfWork (W3.3), domain-event dispatch (W3.4), read service +
/// cache (W3.5), rate providers + Quartz (W3.6), DbContext + host wiring (W3.7).
/// Doc grounding: 01-architecture.md:185 (only LivestockTrading.Api host references
/// module .Infrastructure for DI registration); 03-domain-patterns.md Kural 5.
/// </summary>
public static class CatalogInfrastructureModule
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // W3.0 placeholder — registrations land in W3.3+ (repos/UoW), W3.5 (read+cache),
        // W3.6 (rate providers + Quartz), W3.7 (DbContext + host wiring).
        return services;
    }
}
