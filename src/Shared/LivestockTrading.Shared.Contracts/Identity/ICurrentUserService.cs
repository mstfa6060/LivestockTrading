namespace Shared.Contracts.Identity;

/// <summary>Request-scoped JWT claim okuyucu — DB hit yok, O(1). 05-identity §5 literal 18 üye.</summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    Guid? GetUserId();
    string? GetEmail();
    string? GetDisplayName();
    string? GetGivenName();
    string? GetFamilyName();
    string GetLocale();
    string GetCurrencyCode();
    string GetCountryCode();
    Guid? GetDeviceId();
    AccountType GetAccountType();

    IReadOnlyList<string> GetRoles();
    bool IsInRole(string role);
    bool IsInAnyRole(params string[] roles);

    bool IsEmailVerified();
    bool IsPhoneVerified();

    Guid? GetImpersonationSessionId();
    Guid? GetActualUserId();
}
