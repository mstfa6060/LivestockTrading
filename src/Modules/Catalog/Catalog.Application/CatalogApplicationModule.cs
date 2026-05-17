using FluentValidation;
using LivestockTrading.Catalog.Application.Common;
using LivestockTrading.Catalog.Application.Common.PipelineFilters;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace LivestockTrading.Catalog.Application;

/// <summary>
/// DI extension for Catalog.Application — registers MassTransit.Mediator with
/// pipeline filters (ValidationFilter → handler → UnitOfWorkFilter) and
/// FluentValidation validators from this assembly.
/// Consumer scan and validator scan use CatalogApplicationAssemblyMarker as
/// the assembly reference.
/// IUnitOfWork and IRepository implementations are wired by
/// Catalog.Infrastructure (Wave 3) — this extension only registers Application
/// layer abstractions.
/// </summary>
public static class CatalogApplicationModule
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        var assembly = typeof(CatalogApplicationAssemblyMarker).Assembly;

        services.AddMediator(cfg =>
        {
            cfg.AddConsumers(assembly);
            cfg.ConfigureMediator((context, mcfg) =>
            {
                mcfg.UseConsumeFilter(typeof(ValidationFilter<>), context);
                mcfg.UseConsumeFilter(typeof(UnitOfWorkFilter<>), context);
            });
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
