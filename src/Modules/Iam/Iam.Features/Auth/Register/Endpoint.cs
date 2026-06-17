using FastEndpoints;
using Iam.Domain.Entities;
using Iam.Domain.Errors;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events.Iam;
using Shared.Infrastructure.Messaging;

namespace Iam.Features.Auth.Register;

public sealed class RegisterEndpoint(
    IamDbContext db,
    IPasswordService passwordService,
    IEventPublisher publisher) : Endpoint<RegisterRequest, RegisterResponse>
{
    private static readonly Guid LivestockTradingModuleId = new("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
    private static readonly Guid AdminRoleId = new("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid BuyerRoleId = new("a1000000-0000-0000-0000-000000000006");

    private static readonly string[] AdminEmails =
    [
        "nagehanyazici13@gmail.com",
        "m.mustafaocak@gmail.com",
    ];

    public override void Configure()
    {
        Post("/iam/Auth/Register");
        AllowAnonymous();
        Tags("Auth");
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var emailExists = await db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (emailExists)
        {
            AddError(IamErrors.Users.EmailAlreadyExists);
            await SendErrorsAsync(400, ct);
            return;
        }

        var usernameExists = await db.Users.AnyAsync(u => u.UserName == req.UserName, ct);
        if (usernameExists)
        {
            AddError(IamErrors.Users.UserNameAlreadyExists);
            await SendErrorsAsync(400, ct);
            return;
        }

        var (hash, salt) = passwordService.HashPassword(req.Password);

        var user = new User
        {
            UserName = req.UserName,
            Email = req.Email,
            FirstName = req.FirstName,
            Surname = req.Surname,
            PhoneNumber = req.PhoneNumber,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = true,
            EmailConfirmed = false,
        };

        db.Users.Add(user);

        var roleId = AdminEmails.Contains(req.Email, StringComparer.OrdinalIgnoreCase)
            ? AdminRoleId
            : BuyerRoleId;

        db.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = roleId,
            ModuleId = LivestockTradingModuleId,
        });

        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(UserRegisteredEvent.Subject, new UserRegisteredEvent
        {
            UserId = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            Surname = user.Surname
        }, ct);

        await SendAsync(new RegisterResponse(user.Id, user.Email, user.UserName), 201, ct);
    }
}
