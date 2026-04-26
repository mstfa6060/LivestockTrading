namespace Iam.Features.Users.GetCurrentUser;

public sealed record GetCurrentUserResponse(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string Surname,
    string? PhoneNumber,
    bool IsPhoneVerified,
    bool EmailConfirmed,
    bool IsActive,
    string? City,
    string? District,
    int? CountryId,
    string? Language,
    string? PreferredCurrencyCode,
    Guid? BucketId,
    int UserSource,
    DateTime CreatedAt
);
