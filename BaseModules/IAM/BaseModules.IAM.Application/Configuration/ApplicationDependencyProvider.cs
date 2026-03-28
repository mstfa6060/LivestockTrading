using Arfware.ArfBlocks.Core;
using BaseModules.IAM.Application.Messaging;
using BaseModules.IAM.Infrastructure.Services;
using Common.Services.Caching;
using Common.Services.Messaging;
using Microsoft.Extensions.Configuration;

namespace BaseModules.IAM.Application.Configuration;

public class ApplicationDependencyProvider : ArfBlocksDependencyProvider
{
	//  DÜZELTME: Cache servisleri constructor'dan KALDIRILDI
	public ApplicationDependencyProvider(
		IHttpContextAccessor httpContextAccessor,
		ProjectConfigurations projectConfigurations,
		IConfiguration configuration)
	{
		// Instances
		base.Add<ArfBlocksDependencyProvider>(this);
		base.Add<RelationalDbConfiguration>(projectConfigurations.RelationalDbConfiguration);
		base.Add<EnvironmentConfiguration>(projectConfigurations.EnvironmentConfiguration);
		base.Add<ExternalAuthConfiguration>(projectConfigurations.ExternalAuth);
		base.Add<IHttpContextAccessor>(httpContextAccessor);
		base.Add<CurrentUserModel>(new CurrentUserModel());
		base.Add<IConfiguration>(configuration);

		// Types
		base.Add<EnvironmentService>();
		base.Add<HttpService>();
		base.Add<CurrentUserService>();
		base.Add<DefinitionDbContextOptions>();
		base.Add<DefinitionDbContext>();
		base.Add<IamDbContextOptions>();
		base.Add<IamDbContext>();
		base.Add<IamDbValidationService>();
		base.Add<IamDbVerificationService>();

		base.Add<IJwtService, JwtService>();
		base.Add<ICacheService, CacheService>();

		// GeoIP Service (IP → Country detection)
		base.Add<GeoIpService>();

		// For Authorization Operations
		base.Add<AuthorizationService>();
		base.Add<DbContext, IamDbContext>();

		// Communication
		base.Add<ArfBlocksCommunicator>();
		base.Add<IRabbitMqPublisher>(new RabbitMqPublisher());
		base.Add<ForgotPasswordEmailPublisher>();
	}
}