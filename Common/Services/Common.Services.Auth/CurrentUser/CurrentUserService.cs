using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using Common.Definitions.Base.Enums;
using Arfware.ArfBlocks.Core;
using Common.Definitions.Domain.Models;
using Common.Services.Environment;
using Common.Definitions.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Services.Auth.JsonWebToken;

namespace Common.Services.Auth.CurrentUser;

public class CurrentUserService
{
    private CurrentUserModel _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(ArfBlocksDependencyProvider dependencyProvider)
    {
        _httpContextAccessor = dependencyProvider.GetInstance<IHttpContextAccessor>();
        var environmentService = dependencyProvider.GetInstance<EnvironmentService>();

        if (environmentService.Environment == CustomEnvironments.Test)
        {
            this._currentUser = dependencyProvider.GetInstance<CurrentUserModel>();
        }
        else
        {
            try
            {
                var authorizationValue = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(authorizationValue) && authorizationValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    string jwtInput = authorizationValue.Substring("Bearer ".Length).Trim();

                    jwtInput = jwtInput.Trim('"'); // <<< Önemli: çift tırnakları temizliyoruz


                    var jwtHandler = new JwtSecurityTokenHandler();
                    if (!jwtHandler.CanReadToken(jwtInput))
                    {
                        Console.WriteLine($"Cannot read JWT Token: {jwtInput}");
                        throw new Exception("Invalid JWT format.");
                    }

                    this._currentUser = this.ParseJwt(jwtInput);
                }
                else
                {
                    Console.WriteLine("Authorization header is missing or does not start with Bearer.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"CurrentUserService initialization error: {ex.Message}");
            }
        }
    }

    private CurrentUserModel ParseJwt(string jwtInput)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtService.Secret)),
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidAudience = JwtService.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30) // İsteğe bağlı tolerans süresi
            };

            // Bu metod token'ı doğrular ve claims'i döndürür
            tokenHandler.ValidateToken(jwtInput, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            // 🆕 JWT'den rolleri çıkar
            var roles = jwtToken.Claims
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            Console.WriteLine($"ParseJWT - Parsed Roles: {string.Join(", ", roles)}");

            return new CurrentUserModel
            {
                Id = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? Guid.Empty.ToString()),
                Username = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
                DisplayName = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value,
                TenantId = Guid.Parse(
                    jwtToken.Claims.FirstOrDefault(c =>
                        c.Type == "tenantId" || c.Type == "companyId")?.Value ?? Guid.Empty.ToString()),
                Platform = (ClientPlatforms)Convert.ToInt32(jwtToken.Claims.FirstOrDefault(c => c.Type == "platform")?.Value ?? "3"),
                UserSource = (UserSources)Convert.ToInt32(jwtToken.Claims.FirstOrDefault(c => c.Type == "userSource")?.Value ?? "0"),
                RefreshTokenId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "refreshTokenId")?.Value ?? Guid.Empty.ToString()),
                Roles = roles  // 🆕 Rolleri ekle
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JWT Parsing Error: {ex.Message}");
            return null;
        }
    }


    public Guid GetCurrentRefreshTokenId() => _currentUser?.RefreshTokenId ?? Guid.Empty;
    public Guid GetCurrentUserId() => _currentUser?.Id ?? Guid.Empty;
    // TenantId'yi doğrudan döndürmek için
    public Guid GetTenantId() => _currentUser?.TenantId ?? Guid.Empty;

    // Alias: GetCompanyId() aslında GetTenantId()'yi döndürüyor
    public Guid GetCompanyId() => this.GetTenantId();

    public string GetCurrentUserDisplayName() => _currentUser?.DisplayName;
    public string GetCurrentUserUserName() => _currentUser?.Username;
    public ClientPlatforms GetCurrentUserPlatform() => _currentUser?.Platform ?? ClientPlatforms.Unknown;
    public UserSources GetCurrentUserSource() => _currentUser?.UserSource ?? UserSources.Unknown;

    // 🆕 ADIM 10: Rolleri getir
    public List<string> GetUserRoles() => _currentUser?.Roles ?? new List<string>();

    // 🆕 ADIM 10: Belirli bir modüldeki rolleri getir
    public List<string> GetUserRolesForModule(string moduleName)
    {
        if (_currentUser?.Roles == null)
            return new List<string>();

        return _currentUser.Roles
            .Where(r => r.StartsWith($"{moduleName}.", StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Substring(moduleName.Length + 1)) // "AnimalMarket.Admin" -> "Admin"
            .ToList();
    }

    // 🆕 ADIM 10: Kullanıcının belirli bir modülde belirli bir rolü var mı?
    public bool HasRoleInModule(string moduleName, string roleName)
    {
        if (_currentUser?.Roles == null)
            return false;

        var fullRoleName = $"{moduleName}.{roleName}";
        return _currentUser.Roles.Contains(fullRoleName, StringComparer.OrdinalIgnoreCase);
    }

    public IPAddress GetCurrentUserIp()
    {
        IPAddress remoteIpAddress = null;
        var forwardedFor = _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
            foreach (var ip in ips)
            {
                if (IPAddress.TryParse(ip, out var address) &&
                    (address.AddressFamily == AddressFamily.InterNetwork || address.AddressFamily == AddressFamily.InterNetworkV6))
                {
                    remoteIpAddress = address;
                    break;
                }
            }
        }
        else
        {
            remoteIpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress;
        }

        return remoteIpAddress;
    }

    public CurrentUserModel GetCurrentUser()
    {
        return this._currentUser;
    }
}