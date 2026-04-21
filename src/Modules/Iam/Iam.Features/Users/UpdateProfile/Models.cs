namespace Iam.Features.Users.UpdateProfile;

public sealed record UpdateProfileRequest(
    string FirstName,
    string Surname,
    string? PhoneNumber,
    DateTime? BirthDate,
    string? City,
    string? District,
    int? CountryId,
    string? Language,
    string? PreferredCurrencyCode,
    string? Description
);

public sealed record UpdateProfileResponse(bool Success);
