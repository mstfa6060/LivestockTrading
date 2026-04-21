using System.Security.Claims;
using FastEndpoints;
using Iam.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iam.Features.Users.UpdateProfile;

public sealed class UpdateProfileEndpoint(IamDbContext db) : Endpoint<UpdateProfileRequest, UpdateProfileResponse>
{
    public override void Configure()
    {
        Put("/Users/Me");
        Tags("Users");
    }

    public override async Task HandleAsync(UpdateProfileRequest req, CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        user.FirstName = req.FirstName;
        user.Surname = req.Surname;
        user.PhoneNumber = req.PhoneNumber;
        user.BirthDate = req.BirthDate;
        user.City = req.City;
        user.District = req.District;
        user.CountryId = req.CountryId;
        user.Language = req.Language;
        user.PreferredCurrencyCode = req.PreferredCurrencyCode;
        user.Description = req.Description;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        await SendAsync(new UpdateProfileResponse(true), 200, ct);
    }
}
