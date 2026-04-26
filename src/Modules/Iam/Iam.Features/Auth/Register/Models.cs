namespace Iam.Features.Auth.Register;

public sealed record RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string Surname,
    string? PhoneNumber,
    int Platform
);

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string UserName
);
