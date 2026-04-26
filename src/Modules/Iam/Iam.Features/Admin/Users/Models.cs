namespace Iam.Features.Admin.Users;

public record AdminUserListItem(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string Surname,
    string? PhoneNumber,
    bool IsActive,
    bool EmailConfirmed,
    DateTime CreatedAt);

public record BanUserRequest(Guid Id);
public record UnbanUserRequest(Guid Id);
public record GetAdminUserRequest(Guid Id);
