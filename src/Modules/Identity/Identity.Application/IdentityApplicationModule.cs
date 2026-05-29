using FluentValidation;
using LivestockTrading.Identity.Application.Common;
using LivestockTrading.Identity.Application.Common.PipelineFilters;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LivestockTrading.Identity.Application;

/// <summary>
/// DI extension for Identity.Application — registers MassTransit.Mediator with
/// pipeline filters (ValidationFilter → handler → UnitOfWorkFilter) and
/// FluentValidation validators from this assembly.
/// Consumer scan and validator scan use IdentityApplicationAssemblyMarker as
/// the assembly reference.
/// TimeProvider.System registered as singleton (BCL clock abstraction, B-W4.2-1).
/// IUnitOfWork and IRepository implementations are wired by
/// Identity.Infrastructure (Wave 4 W4.3) — this extension only registers Application
/// layer abstractions.
/// </summary>
public static class IdentityApplicationModule
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        var assembly = typeof(IdentityApplicationAssemblyMarker).Assembly;

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

        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}
