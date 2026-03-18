using Common.Services.Caching;
using Common.Services.Messaging;
using LivestockTrading.Application.Authorization;
using LivestockTrading.Application.Services;
using LivestockTrading.Infrastructure.RelationalDB;
using LivestockTrading.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace LivestockTrading.Application.Configuration;

public class ApplicationDependencyProvider : ArfBlocksDependencyProvider
{
    public ApplicationDependencyProvider(
        IHttpContextAccessor httpContextAccessor,
        ProjectConfigurations projectConfigurations,
        IConfiguration configuration)
    {
        // Instances
        base.Add<ArfBlocksDependencyProvider>(this);
        base.Add<RelationalDbConfiguration>(projectConfigurations.RelationalDbConfiguration);
        base.Add<EnvironmentConfiguration>(projectConfigurations.EnvironmentConfiguration);
        base.Add<IHttpContextAccessor>(httpContextAccessor);
        base.Add<CurrentUserModel>(new CurrentUserModel());
        base.Add<IConfiguration>(configuration);

        // Types
        base.Add<CurrentUserService>();
        base.Add<EnvironmentService>();

        // Data
        base.Add<DefinitionDbContextOptions>();
        base.Add<LivestockTradingDbContextOptions>();
        base.Add<LivestockTradingModuleDbContext>();
        base.Add<DbContext, LivestockTradingModuleDbContext>();

        // Communication
        base.Add<ArfBlocksCommunicator>();
        base.Add<ICacheService, CacheService>();
        base.Add<AuthorizationService>();
        base.Add<IRabbitMqPublisher>(new RabbitMqPublisher());

        // Services
        base.Add<LivestockTradingModuleDbVerificationService>();
        base.Add<LivestockTradingModuleDbValidationService>();
        base.Add<PresenceService>();
        base.Add<SubscriptionEnforcementService>();

        // Authorization
        base.Add<PermissionService>();

        // Translation
        base.Add<AutoTranslationService>();
    }
}
