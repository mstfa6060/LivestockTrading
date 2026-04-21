using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Common.Definitions.Base.Enums;
using Common.Definitions.Domain.Entities;
using System.Security.Cryptography;

namespace Common.Services.Auth.JsonWebToken;

public class JwtService : IJwtService
{
	public static string Secret = "Rj!7dXrQ*5z9@Lb^PqKf!M2&gUw#AeZx7Vp$ChN+36YT@tW%";
	public static string Audience = "api.hirovo.com";
	private readonly int WebExpirationDayCount = 7;
	private readonly int MobileExpirationDayCount = 7;
	private readonly int ExpirationMinutesForTempJwt = 5;

	// 🆕 ADIM 8: roles parametresi eklendi
	public string GenerateJwt(
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
		List<string> roles = null  // 🆕 Yeni parametre
	)
	{
		try
		{
			Console.WriteLine($"JWT Generation Debug - UserId: {userId}, TenantId: {tenantId}, UserName: {userName}, DisplayName: {displayName}");

			// 🆕 Rolleri logla
			if (roles != null && roles.Any())
			{
				Console.WriteLine($"JWT Generation Debug - Roles: {string.Join(", ", roles)}");
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(JwtService.Secret);

			// 🆕 Claim listesini dinamik olarak oluştur
			var claims = new List<Claim>
			{
				new Claim("nameid", userId.ToString()),
				new Claim("given_name", userName ?? ""),
				new Claim("unique_name", displayName ?? ""),
				new Claim("email", email ?? ""),
				new Claim("bucketId", bucketId ?? ""),
				new Claim("tenantId", tenantId.ToString()),
				new Claim("platform", $"{((decimal)platform)}"),
				new Claim("userSource", $"{((decimal)userSource)}"),
				new Claim("refreshTokenId", refreshTokenId.ToString()),
				new Claim("companyId", tenantId.ToString())
			};

			// 🆕 ADIM 8: Rolleri JWT claims'e ekle
			if (roles != null && roles.Any())
			{
				foreach (var role in roles)
				{
					claims.Add(new Claim("role", role));
				}
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = JwtService.Audience,
				IssuedAt = DateTime.Now,
				Subject = new ClaimsIdentity(claims),  // 🆕 Dinamik claims listesi kullan
				Expires = expiresAt,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var jwt = tokenHandler.WriteToken(token);

			Console.WriteLine($"JWT Generation Debug - Success, Length: {jwt?.Length ?? 0}");

			return jwt;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"JWT Generation Error: {ex.Message}");
			Console.WriteLine($"JWT Generation StackTrace: {ex.StackTrace}");
			return null;
		}
	}

	public int GetExpirationDayCount()
	{
		return this.WebExpirationDayCount;
	}

	public string GenerateSecureRefreshToken(int size = 64)
	{
		var randomBytes = new byte[size];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomBytes);
		return Convert.ToBase64String(randomBytes);
	}
}