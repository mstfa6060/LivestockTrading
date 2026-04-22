using System.Security.Claims;
using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Users.GetCurrentUser;

public sealed class GetCurrentUserEndpoint(IamDbContext db) : Endpoint<GetCurrentUserRequest, GetCurrentUserResponse>
{
    public override void Configure()
    {
        Post("/iam/Users/Me");
        Tags("Users");
    }

    public override async Task HandleAsync(GetCurrentUserRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new GetCurrentUserResponse(
            user.Id,
            user.UserName,
            user.Email,
            user.FirstName,
            user.Surname,
            user.PhoneNumber,
            user.IsPhoneVerified,
            user.EmailConfirmed,
            user.IsActive,
            user.City,
            user.District,
            user.CountryId,
            user.Language,
            user.PreferredCurrencyCode,
            user.BucketId,
            (int)user.UserSource,
            user.CreatedAt
        ), 200, ct);
    }
}
