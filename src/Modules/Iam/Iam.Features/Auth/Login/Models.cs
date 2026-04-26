namespace Iam.Features.Auth.Login;

public sealed record LoginRequest(
    string? Provider,
    string? UserName,
    string? Email,
    string? Password,
    string? ExternalProviderUserId,
    string? FirstName,
    string? Surname,
    string? PhoneNumber,
    DateTime? BirthDate,
    int Platform
);

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    string UserName,
    string FirstName,
    string Surname,
    string AuthProvider
);
