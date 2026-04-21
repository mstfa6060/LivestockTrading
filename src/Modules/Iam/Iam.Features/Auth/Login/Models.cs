namespace Iam.Features.Auth.Login;

public sealed record LoginRequest(
    string Email,
    string Password,
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
    string Surname
);
