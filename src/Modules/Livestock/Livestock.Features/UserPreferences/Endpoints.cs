using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.UserPreferences;

public class GetUserPreferencesEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<UserPreferenceDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/UserPreferences/Get");
        Tags("UserPreferences");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var pref = await db.UserPreferences.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == user.UserId && !p.IsDeleted, ct);

        if (pref is null)
        {
            pref = new UserPreference { UserId = user.UserId };
            await SendAsync(UserPreferenceMapper.Map(pref), 200, ct);
            return;
        }

        await SendAsync(UserPreferenceMapper.Map(pref), 200, ct);
    }
}

public class UpsertUserPreferencesEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpsertUserPreferenceRequest, UserPreferenceDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/UserPreferences/Update");
        Tags("UserPreferences");
    }

    public override async Task HandleAsync(UpsertUserPreferenceRequest req, CancellationToken ct)
    {
        var pref = await db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == user.UserId && !p.IsDeleted, ct);

        if (pref is null)
        {
            pref = new UserPreference { UserId = user.UserId };
            db.UserPreferences.Add(pref);
        }

        pref.PreferredCurrency = req.PreferredCurrency;
        pref.PreferredLanguage = req.PreferredLanguage;
        pref.CountryCode = req.CountryCode;
        pref.TimeZone = req.TimeZone;
        pref.EmailNotificationsEnabled = req.EmailNotificationsEnabled;
        pref.SmsNotificationsEnabled = req.SmsNotificationsEnabled;
        pref.PushNotificationsEnabled = req.PushNotificationsEnabled;
        pref.DarkModeEnabled = req.DarkModeEnabled;
        pref.ProductsPerPage = req.ProductsPerPage;
        pref.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await SendAsync(UserPreferenceMapper.Map(pref), 200, ct);
    }
}

file static class UserPreferenceMapper
{
    internal static UserPreferenceDetail Map(UserPreference p) => new(
        p.Id, p.UserId, p.PreferredCurrency, p.PreferredLanguage,
        p.CountryCode, p.TimeZone,
        p.EmailNotificationsEnabled, p.SmsNotificationsEnabled, p.PushNotificationsEnabled,
        p.DarkModeEnabled, p.ProductsPerPage, p.CreatedAt);
}
