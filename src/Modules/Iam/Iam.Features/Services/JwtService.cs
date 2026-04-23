using System.Security.Claims;
using System.Security.Cryptography;
using FastEndpoints.Security;
using Iam.Domain.Entities;
using Iam.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Iam.Features.Services;

public sealed class JwtService : IJwtService
{
    private readonly string _signingKey;
    private readonly int _accessTokenMinutes;

    public JwtService(IConfiguration configuration)
    {
        _signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");
        _accessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
    }

    public string GenerateAccessToken(User user, IEnumerable<string> roles, ClientPlatforms platform, Guid refreshTokenId)
    {
        return JwtBearer.CreateToken(o =>
        {
            o.SigningKey = _signingKey;
            o.ExpireAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

            foreach (var role in roles)
            {
                o.User.Roles.Add(role);
            }

            o.User.Claims.Add(new(ClaimTypes.NameIdentifier, user.Id.ToString()));
            o.User.Claims.Add(new(ClaimTypes.Email, user.Email));
            o.User.Claims.Add(new(ClaimTypes.Name, $"{user.FirstName} {user.Surname}"));
            o.User.Claims.Add(new("username", user.UserName));
            o.User.Claims.Add(new("platform", ((int)platform).ToString()));
            o.User.Claims.Add(new("user_source", ((int)user.UserSource).ToString()));
            o.User.Claims.Add(new("refresh_token_id", refreshTokenId.ToString()));
            o.User.Claims.Add(new("bucket_id", user.BucketId?.ToString() ?? string.Empty));
        });
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
