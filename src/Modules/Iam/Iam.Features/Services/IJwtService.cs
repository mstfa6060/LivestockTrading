using Iam.Domain.Entities;
using Iam.Domain.Enums;

namespace Iam.Features.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles, ClientPlatforms platform, Guid refreshTokenId);
    string GenerateRefreshToken();
}
