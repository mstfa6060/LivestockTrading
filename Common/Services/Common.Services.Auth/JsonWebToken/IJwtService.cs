using Common.Definitions.Base.Enums;
using Common.Definitions.Domain.Entities;

namespace Common.Services.Auth.JsonWebToken;

public interface IJwtService
{
	string GenerateJwt(
	 Guid userId,
	 Guid tenantId,
	 string userName,
	 string displayName,
	 string email,
	 string bucketId,
	 ClientPlatforms platform,
	 DateTime expiresAt,
	 UserSources userSource,
	 Guid refreshTokenId,
	 List<string> roles = null);

	int GetExpirationDayCount();
	string GenerateSecureRefreshToken(int size = 64);
}