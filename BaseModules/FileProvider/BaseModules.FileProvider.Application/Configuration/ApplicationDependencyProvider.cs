using BaseModules.FileProvider.Infrastructure.RelationalDB;
using Common.Definitions.Domain.Models;
using Common.Services.Auth.Authorization;
using Common.Services.Caching;
using Common.Services.FileOperations.ImageProcessing;
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

		// MinIO varsa MinIO kullan, yoksa filesystem fallback
		var minioEndpoint = configuration["MinIO:Endpoint"];
		if (!string.IsNullOrWhiteSpace(minioEndpoint))
		{
			var minioAccessKey = configuration["MinIO:AccessKey"] ?? "minioadmin";
			var minioSecretKey = configuration["MinIO:SecretKey"] ?? "minioadmin";
			var minioUseSSL = bool.TryParse(configuration["MinIO:UseSSL"], out var ssl) && ssl;
			var minioBucket = configuration["MinIO:BucketName"] ?? "livestocktrading";
			base.Add<IFileStorageService>(new MinIOFileStorageService(minioEndpoint, minioAccessKey, minioSecretKey, minioUseSSL, minioBucket));
			Console.WriteLine($"[FileProvider] Using MinIO storage: {minioEndpoint}/{minioBucket}");
		}
		else
		{
			base.Add<IFileStorageService>(new FileStorageService());
			Console.WriteLine("[FileProvider] Using filesystem storage (MinIO not configured)");
		}
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