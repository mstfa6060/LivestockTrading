namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed record RefreshTokenRequest(string RefreshToken, bool RememberMe);

public sealed record RefreshTokenCommand(RefreshTokenRequest Dto, string IpAddress, string UserAgent);
