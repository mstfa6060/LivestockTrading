namespace Shared.Abstractions.Identity;

public interface IUserContext
{
    Guid UserId { get; }
    string Email { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
