namespace Iam.Features.Auth.RefreshToken;

public sealed record RefreshTokenRequest(string Token);

public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
