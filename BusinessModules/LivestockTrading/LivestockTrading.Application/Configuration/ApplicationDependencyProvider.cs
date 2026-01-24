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
        base.Add<AuthorizationService>();

        // Services
        base.Add<LivestockTradingModuleDbVerificationService>();
        base.Add<LivestockTradingModuleDbValidationService>();
    }
}
