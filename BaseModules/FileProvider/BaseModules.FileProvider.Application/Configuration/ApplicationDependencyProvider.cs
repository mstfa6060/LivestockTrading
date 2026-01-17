using BaseModules.FileProvider.Infrastructure.RelationalDB;
using Common.Definitions.Domain.Models;
using Common.Services.Auth.Authorization;
using Common.Services.Caching;
using Microsoft.Extensions.Configuration;

namespace BaseModules.FileProvider.Application.Configuration;

public class ApplicationDependencyProvider : ArfBlocksDependencyProvider
{
	public ApplicationDependencyProvider(
		IHttpContextAccessor httpContextAccessor,
		ProjectConfigurations projectConfigurations,
		IConfiguration configuration)
	{
		// Instances
		base.Add<RelationalDbConfiguration>(projectConfigurations.RelationalDbConfiguration);
		base.Add<EnvironmentConfiguration>(projectConfigurations.EnvironmentConfiguration);
		base.Add<EnvironmentService>();

		base.Add<DocumentDbOptions>(projectConfigurations.DocumentDbOptions);
		base.Add<IHttpContextAccessor>(httpContextAccessor);
		base.Add<CurrentUserModel>(new CurrentUserModel());
		base.Add<AuditLogService>();
		base.Add<FileProviderDocumentDbValidationService>();
		base.Add<IConfiguration>(configuration);

		// Types
		base.Add<ArfBlocksDependencyProvider>(this);
		base.Add<IFileStorageService>(new FileStorageService());
		base.Add<IJwtService, JwtService>();
		base.Add<FileChangeTrackingService, FileChangeTrackingService>();
		base.Add<DefinitionsDocumentDbContext>();

		base.Add<FileProviderRelationalDbContext>();
		base.Add<CurrentUserService>();
		base.Add<DocxService>();
		base.Add<PdfService>();

		base.Add<DefinitionDbContextOptions>();
		base.Add<DefinitionDbContext>();

		//  Cache Service - CacheService constructor'ı IConfiguration bekliyor
		base.Add<ICacheService, CacheService>();

		// For Authorization Operations
		base.Add<AuthorizationService>();
		base.Add<DbContext, DefinitionDbContext>();
	}
}