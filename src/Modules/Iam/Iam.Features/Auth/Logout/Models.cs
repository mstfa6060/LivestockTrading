namespace Iam.Features.Auth.Logout;

public sealed record LogoutRequest(string RefreshToken);

public sealed record LogoutResponse(bool Success);
