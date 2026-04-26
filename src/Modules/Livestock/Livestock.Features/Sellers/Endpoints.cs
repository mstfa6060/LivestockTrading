using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Sellers;

public class GetAllSellersEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<SellerListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/All");
        AllowAnonymous();
        Tags("Sellers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sellers = await db.Sellers
            .AsNoTracking()
            .Where(s => s.Status == SellerStatus.Active)
            .OrderByDescending(s => s.AverageRating)
            .Select(s => new SellerListItem(
                s.Id, s.UserId, s.BusinessName, s.Status,
                s.Status != SellerStatus.Suspended && s.Status != SellerStatus.Banned && s.Status != SellerStatus.Inactive,
                s.Status == SellerStatus.Active && s.VerifiedAt != null,
                s.AverageRating, s.ReviewCount, s.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(sellers, 200, ct);
    }
}

public class GetSellerEndpoint(LivestockDbContext db) : Endpoint<GetSellerRequest, SellerDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/Detail");
        AllowAnonymous();
        Tags("Sellers");
    }

    public override async Task HandleAsync(GetSellerRequest req, CancellationToken ct)
    {
        var s = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (s is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new SellerDetail(s.Id, s.UserId, s.BusinessName, s.Description, s.PhoneNumber, s.Email, s.WebsiteUrl, s.TaxNumber, s.LogoUrl, s.Status, s.IsActive, s.IsVerified, s.AverageRating, s.ReviewCount, s.VerifiedAt, s.CreatedAt), 200, ct);
    }
}

public class GetMySellerEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<SellerDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/Me");
        Tags("Sellers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var s = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.UserId, ct);
        if (s is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new SellerDetail(s.Id, s.UserId, s.BusinessName, s.Description, s.PhoneNumber, s.Email, s.WebsiteUrl, s.TaxNumber, s.LogoUrl, s.Status, s.IsActive, s.IsVerified, s.AverageRating, s.ReviewCount, s.VerifiedAt, s.CreatedAt), 200, ct);
    }
}

public class BecomeSellerEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<BecomeSellerRequest, SellerDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/Register");
        Tags("Sellers");
    }

    public override async Task HandleAsync(BecomeSellerRequest req, CancellationToken ct)
    {
        var exists = await db.Sellers.AnyAsync(s => s.UserId == user.UserId, ct);
        if (exists)
        {
            AddError(LivestockErrors.SellerErrors.SellerAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var seller = new Seller
        {
            UserId = user.UserId,
            BusinessName = req.BusinessName,
            Description = req.Description,
            PhoneNumber = req.PhoneNumber,
            Email = req.Email,
            WebsiteUrl = req.WebsiteUrl,
            TaxNumber = req.TaxNumber,
            Status = SellerStatus.PendingVerification
        };

        db.Sellers.Add(seller);
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(SellerRegisteredEvent.Subject, new SellerRegisteredEvent
        {
            SellerId = seller.Id,
            UserId = seller.UserId,
            BusinessName = seller.BusinessName
        }, ct);

        await SendAsync(new SellerDetail(seller.Id, seller.UserId, seller.BusinessName, seller.Description, seller.PhoneNumber, seller.Email, seller.WebsiteUrl, seller.TaxNumber, seller.LogoUrl, seller.Status, seller.IsActive, seller.IsVerified, 0, 0, null, seller.CreatedAt), 201, ct);
    }
}

public class UpdateSellerEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateSellerRequest, SellerDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/UpdateMe");
        Tags("Sellers");
    }

    public override async Task HandleAsync(UpdateSellerRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        seller.BusinessName = req.BusinessName;
        seller.Description = req.Description;
        seller.PhoneNumber = req.PhoneNumber;
        seller.Email = req.Email;
        seller.WebsiteUrl = req.WebsiteUrl;
        seller.TaxNumber = req.TaxNumber;
        seller.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new SellerDetail(seller.Id, seller.UserId, seller.BusinessName, seller.Description, seller.PhoneNumber, seller.Email, seller.WebsiteUrl, seller.TaxNumber, seller.LogoUrl, seller.Status, seller.IsActive, seller.IsVerified, seller.AverageRating, seller.ReviewCount, seller.VerifiedAt, seller.CreatedAt), 200, ct);
    }
}

public class VerifySellerEndpoint(LivestockDbContext db, IEventPublisher publisher) : Endpoint<VerifySellerRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/Verify");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Sellers");
    }

    public override async Task HandleAsync(VerifySellerRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (seller.Status == SellerStatus.Active)
        {
            AddError(LivestockErrors.SellerErrors.SellerAlreadyVerified);
            await SendErrorsAsync(409, ct);
            return;
        }

        seller.Status = SellerStatus.Active;
        seller.VerifiedAt = DateTime.UtcNow;
        seller.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(SellerVerifiedEvent.Subject, new SellerVerifiedEvent
        {
            SellerId = seller.Id,
            UserId = seller.UserId
        }, ct);

        await SendNoContentAsync(ct);
    }
}

public class GetNearbySellersEndpoint(LivestockDbContext db) : Endpoint<NearbySellersRequest, List<NearbySellerItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/Nearby");
        AllowAnonymous();
        Tags("Sellers");
    }

    public override async Task HandleAsync(NearbySellersRequest req, CancellationToken ct)
    {
        // PostGIS expects (X=lon, Y=lat) and SRID 4326 for geography. Center
        // built once and reused so EF emits a single parameter binding.
        var center = new Point(req.Longitude, req.Latitude) { SRID = 4326 };
        var radiusMeters = req.RadiusKm * 1000;

        // Querying Sellers directly so the GIST index on Sellers.Geo can drive
        // the plan via ST_DWithin / <-> (KNN). Note: the LINQ form matters —
        // EF maps `IsWithinDistance` → `ST_DWithin` and `Distance(...)` in
        // ORDER BY → the `<->` KNN operator. Anything else (e.g. comparing
        // ST_Distance to a literal in WHERE) would force a Seq Scan.
        var query = db.Sellers
            .AsNoTracking()
            .Where(s => !s.IsDeleted
                     && s.Status == SellerStatus.Active
                     && s.VerifiedAt != null
                     && s.Geo != null
                     && s.Geo!.IsWithinDistance(center, radiusMeters));

        // City / CountryCode aren't on Seller — LEFT JOIN the seller's primary
        // Location row (OwnerId = Seller.Id, OwnerType = "Seller"). Picking
        // the first match keeps the projection EF-translatable; if a seller
        // has multiple locations the response just shows one.
        // Geo itself is selected (not .X/.Y) because ST_X/ST_Y aren't defined
        // for `geography` — we read the Point's coordinates client-side.
        var projected = query
            .Select(s => new
            {
                s.Id,
                s.UserId,
                s.BusinessName,
                s.LogoUrl,
                s.Status,
                s.VerifiedAt,
                s.AverageRating,
                s.ReviewCount,
                Loc = db.Locations
                    .Where(l => !l.IsDeleted
                             && l.OwnerId == s.Id
                             && l.OwnerType == "Seller")
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(l => new { l.City, l.CountryCode })
                    .FirstOrDefault(),
                Geo = s.Geo!,
                DistanceMeters = s.Geo!.Distance(center),
            });

        // Optional country filter: prefer the matched Location's CountryCode.
        if (!string.IsNullOrWhiteSpace(req.CountryCode))
        {
            projected = projected.Where(x => x.Loc != null && x.Loc.CountryCode == req.CountryCode);
        }

        var rows = await projected
            .OrderBy(x => x.DistanceMeters)  // <-> KNN ordering
            .Take(req.Limit)
            .ToListAsync(ct);

        var nearby = rows
            .Select(s => new NearbySellerItem(
                s.Id,
                s.UserId,
                s.BusinessName,
                s.LogoUrl,
                s.Status == SellerStatus.Active && s.VerifiedAt != null,
                s.Status,
                s.AverageRating,
                s.ReviewCount,
                s.Loc?.City,
                s.Loc?.CountryCode ?? string.Empty,
                s.Geo.Y,    // latitude  (NTS Point: X=lon, Y=lat)
                s.Geo.X,    // longitude
                s.DistanceMeters / 1000.0))
            .ToList();

        await SendAsync(nearby, 200, ct);
    }

    [Obsolete("Replaced by PostGIS ST_DWithin / KNN — kept for one release window so callers " +
              "comparing distance values aren't surprised by tiny rounding differences. " +
              "Remove two releases after MST-72 ships.", error: false)]
    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    [Obsolete("Helper for the obsolete HaversineKm — see that method.", error: false)]
    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}

public class SuspendSellerEndpoint(LivestockDbContext db) : Endpoint<SuspendSellerRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/Suspend");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Sellers");
    }

    public override async Task HandleAsync(SuspendSellerRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (seller.Status == SellerStatus.Suspended)
        {
            AddError(LivestockErrors.SellerErrors.SellerAlreadySuspended);
            await SendErrorsAsync(409, ct);
            return;
        }

        seller.Status = SellerStatus.Suspended;
        seller.SuspensionReason = req.Reason;
        seller.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
