namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed record LogoutRequest(string RefreshToken);

public sealed record LogoutCommand(Guid UserId, LogoutRequest Dto);
