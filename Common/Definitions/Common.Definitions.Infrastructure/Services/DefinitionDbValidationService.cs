
using System.Text.Json;
using Arfware.ArfBlocks.Core.Exceptions;
using Common.Definitions.Domain.Errors;
using Common.Services.ErrorCodeGenerator;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;

namespace Common.Definitions.Infrastructure.Services;


public class DefinitionDbValidationService
{
    private readonly DefinitionDbContext _dbContext;

    public DefinitionDbValidationService(DefinitionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ValidateRoleExist(string roleName)
    {
        // Get
        var authorizedModuleExistById = await _dbContext.AppRoles.AnyAsync(d => d.Name.ToLower() == roleName.ToLower());

        // Check
        if (authorizedModuleExistById)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.RoleErrors.NameValid));
    }

    public async Task ValidateRoleExist(Guid roleId)
    {
        var roleExist = await _dbContext.AppRoles.AnyAsync(d => d.Id == roleId);

        if (!roleExist)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.RoleErrors.IdValid));
    }


    public async Task ValidateUserByUserNameExist(string userName)
    {
        // Get
        var authorizedModuleExistById = await _dbContext.AppUsers.AnyAsync(d => d.UserName.ToLower() == userName.ToLower());

        // Check
        if (authorizedModuleExistById)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNameValid));
    }

    public async Task ValidateUserByEmailExist(string email)
    {
        // Get
        var authorizedModuleExistById = await _dbContext.AppUsers.AnyAsync(d => d.Email.ToLower() == email.ToLower());

        // Check
        if (authorizedModuleExistById)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.EmailValid));
    }

    public async Task ValidateUserByEmailNotFound(string email)
    {
        // Get
        var authorizedModuleExistById = await _dbContext.AppUsers.AnyAsync(d => d.Email.ToLower() == email.ToLower());

        // Check
        if (!authorizedModuleExistById)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.EmailNotFound));
    }

    // public async Task ValidateUserByPhoneExist(string phoneNumber)
    // {
    //     // Get
    //     var authorizedModuleExistById = await _dbContext.AppUsers.AnyAsync(d => d.PhoneNumber == phoneNumber);

    //     // Check
    //     if (authorizedModuleExistById)
    //         throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.PhoneNumberValid));
    // }

    public async Task<string> ValidateGoogleToken(string idToken, string googleClientId)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }
            });
        }
        catch (InvalidJwtException ex)
        {
            Console.WriteLine($"❌ Google JWT doğrulama hatası: {ex.Message}");
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.GoogleTokenInvalid));
        }

        Console.WriteLine($" Google kullanıcı: {payload.Email}, verified: {payload.EmailVerified}");

        if (!payload.EmailVerified)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.GoogleEmailNotVerified));

        return payload.Subject; // Google kullanıcı benzersiz ID
    }


    //public async Task<string> ValidateGoogleToken(string token, string googleClientId)
    //{
    //    using var httpClient = new HttpClient();

    //    var requestUri = $"https://oauth2.googleapis.com/tokeninfo?id_token={token}";
    //    var response = await httpClient.GetAsync(requestUri);
    //    Console.WriteLine($"Google Token Validation Response: {response}");


    //    if (!response.IsSuccessStatusCode)
    //        return null;

    //    var content = await response.Content.ReadAsStringAsync();
    //    var payload = JsonSerializer.Deserialize<GoogleTokenPayload>(content, new JsonSerializerOptions
    //    {
    //        PropertyNameCaseInsensitive = true
    //    });

    //    Console.WriteLine($"Payload Response {content}");

    //    //  exp kontrolü burada
    //    if (long.TryParse(payload?.Exp, out var expUnix))
    //    {
    //        var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

    //        if (expirationDate < DateTime.UtcNow)
    //            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.GoogleTokenExpired));
    //    }
    //    Console.WriteLine($"Google Token Expiration: {payload?.Exp} (UTC: {DateTimeOffset.FromUnixTimeSeconds(long.Parse(payload.Exp)).UtcDateTime})");
    //    Console.WriteLine($"Google ClientId {googleClientId}");
    //    //  aud (client ID) doğrulaması
    //    if (payload?.Aud != googleClientId)
    //        return null;

    //    return payload.Sub; // Google user ID (benzersiz ID)
    //}

    public async Task<string> ValidateAppleToken(string token, string appleClientId, string appleClientSecret)
    {
        using var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://appleid.apple.com/auth/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", appleClientId },
                { "client_secret", appleClientSecret },
                { "grant_type", "authorization_code" },
                { "code", token }
            })
        };

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<AppleTokenPayload>(content);

        return tokenData?.IdTokenPayload?.Sub;
    }

    public async Task ValidateUserRoleExist(Guid userRoleId)
    {
        var isUserRoleExist = await _dbContext.UserRoles.AnyAsync(a => a.Id == userRoleId);

        // Check
        if (!isUserRoleExist)
            throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.RoleErrors.RoleNotExist));
    }

}