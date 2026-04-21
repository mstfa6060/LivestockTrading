using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shared.Abstractions.Identity;

namespace Shared.Infrastructure.Identity;

public sealed class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public Guid UserId =>
        Guid.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : Guid.Empty;

    public string Email =>
        Principal?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public IReadOnlyList<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) =>
        Principal?.IsInRole(role) ?? false;
}
