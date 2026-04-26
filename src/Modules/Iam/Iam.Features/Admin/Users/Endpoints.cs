using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Admin.Users;

public class ListUsersEndpoint(IamDbContext db) : EndpointWithoutRequest<List<AdminUserListItem>>
{
    public override void Configure()
    {
        Post("/iam/Admin/Users/All");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var users = await db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserListItem(
                u.Id, u.UserName, u.Email, u.FirstName, u.Surname,
                u.PhoneNumber, u.IsActive, u.EmailConfirmed, u.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(users, 200, ct);
    }
}

public class BanUserEndpoint(IamDbContext db) : Endpoint<BanUserRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/iam/Admin/Users/Ban");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(BanUserRequest req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == req.Id, ct);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!user.IsActive)
        {
            await SendAsync(new EmptyResponse(), 409, ct);
            return;
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class UnbanUserEndpoint(IamDbContext db) : Endpoint<UnbanUserRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/iam/Admin/Users/Unban");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(UnbanUserRequest req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == req.Id, ct);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (user.IsActive)
        {
            await SendAsync(new EmptyResponse(), 409, ct);
            return;
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
