using FastEndpoints;
using Iam.Domain.Entities;
using Iam.Domain.Errors;
using Iam.Features.Services;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Auth.Register;

public sealed class RegisterEndpoint(
    IamDbContext db,
    IPasswordService passwordService) : Endpoint<RegisterRequest, RegisterResponse>
{
    public override void Configure()
    {
        Post("/Auth/Register");
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

        // Assign default Buyer role for LivestockTrading module
        var buyerRoleId = new Guid("a1000000-0000-0000-0000-000000000006");
        var moduleId = new Guid("DFD018C9-FC32-42C4-AEFD-70A5942A295E");

        db.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = buyerRoleId,
            ModuleId = moduleId,
        });

        await db.SaveChangesAsync(ct);

        await SendAsync(new RegisterResponse(user.Id, user.Email, user.UserName), 201, ct);
    }
}
