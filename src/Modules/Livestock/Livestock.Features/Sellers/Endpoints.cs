using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
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
        var query = db.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted
                     && p.Status == ProductStatus.Active
                     && p.LocationId != null
                     && p.Location!.Latitude != null
                     && p.Location.Longitude != null
                     && p.Seller.Status == SellerStatus.Active);

        if (!string.IsNullOrWhiteSpace(req.CountryCode))
        {
            query = query.Where(p => p.Location!.CountryCode == req.CountryCode);
        }

        var rows = await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(5000)
            .Select(p => new
            {
                p.SellerId,
                p.Seller.UserId,
                p.Seller.BusinessName,
                p.Seller.LogoUrl,
                p.Seller.Status,
                p.Seller.VerifiedAt,
                p.Seller.AverageRating,
                p.Seller.ReviewCount,
                p.Location!.City,
                p.Location.CountryCode,
                Latitude = p.Location.Latitude!.Value,
                Longitude = p.Location.Longitude!.Value,
            })
            .ToListAsync(ct);

        var nearby = rows
            .GroupBy(r => r.SellerId)
            .Select(g => g.First())
            .Select(s => new NearbySellerItem(
                s.SellerId,
                s.UserId,
                s.BusinessName,
                s.LogoUrl,
                s.Status == SellerStatus.Active && s.VerifiedAt != null,
                s.Status,
                s.AverageRating,
                s.ReviewCount,
                s.City,
                s.CountryCode,
                s.Latitude,
                s.Longitude,
                HaversineKm(req.Latitude, req.Longitude, s.Latitude, s.Longitude)))
            .OrderBy(s => s.DistanceKm)
            .Take(req.Limit)
            .ToList();

        await SendAsync(nearby, 200, ct);
    }

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
