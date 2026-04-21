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
        Get("/Sellers");
        AllowAnonymous();
        Tags("Sellers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sellers = await db.Sellers
            .AsNoTracking()
            .Where(s => s.Status == SellerStatus.Active)
            .OrderByDescending(s => s.AverageRating)
            .Select(s => new SellerListItem(s.Id, s.UserId, s.BusinessName, s.Status, s.AverageRating, s.ReviewCount, s.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(sellers, 200, ct);
    }
}

public class GetSellerEndpoint(LivestockDbContext db) : Endpoint<GetSellerRequest, SellerDetail>
{
    public override void Configure()
    {
        Get("/Sellers/{Id}");
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

        await SendAsync(new SellerDetail(s.Id, s.UserId, s.BusinessName, s.Description, s.PhoneNumber, s.Email, s.WebsiteUrl, s.TaxNumber, s.LogoUrl, s.Status, s.AverageRating, s.ReviewCount, s.VerifiedAt, s.CreatedAt), 200, ct);
    }
}

public class GetMySellerEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<SellerDetail>
{
    public override void Configure()
    {
        Get("/Sellers/Me");
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

        await SendAsync(new SellerDetail(s.Id, s.UserId, s.BusinessName, s.Description, s.PhoneNumber, s.Email, s.WebsiteUrl, s.TaxNumber, s.LogoUrl, s.Status, s.AverageRating, s.ReviewCount, s.VerifiedAt, s.CreatedAt), 200, ct);
    }
}

public class BecomeSellerEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<BecomeSellerRequest, SellerDetail>
{
    public override void Configure()
    {
        Post("/Sellers/Register");
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

        await SendAsync(new SellerDetail(seller.Id, seller.UserId, seller.BusinessName, seller.Description, seller.PhoneNumber, seller.Email, seller.WebsiteUrl, seller.TaxNumber, seller.LogoUrl, seller.Status, 0, 0, null, seller.CreatedAt), 201, ct);
    }
}

public class UpdateSellerEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateSellerRequest, SellerDetail>
{
    public override void Configure()
    {
        Put("/Sellers/Me");
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

        await SendAsync(new SellerDetail(seller.Id, seller.UserId, seller.BusinessName, seller.Description, seller.PhoneNumber, seller.Email, seller.WebsiteUrl, seller.TaxNumber, seller.LogoUrl, seller.Status, seller.AverageRating, seller.ReviewCount, seller.VerifiedAt, seller.CreatedAt), 200, ct);
    }
}

public class VerifySellerEndpoint(LivestockDbContext db, IEventPublisher publisher) : Endpoint<VerifySellerRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Sellers/{Id}/Verify");
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

public class GetNearbySellersEndpoint(LivestockDbContext db) : Endpoint<GetNearbySellersRequest, List<NearbySeller>>
{
    public override void Configure()
    {
        Get("/Sellers/Nearby");
        AllowAnonymous();
        Tags("Sellers");
    }

    public override async Task HandleAsync(GetNearbySellersRequest req, CancellationToken ct)
    {
        var sellerLocations = await db.Locations
            .AsNoTracking()
            .Where(l => l.OwnerType == "Seller" && l.Latitude.HasValue && l.Longitude.HasValue)
            .Select(l => new { l.OwnerId, l.Latitude, l.Longitude })
            .ToListAsync(ct);

        var sellerIds = sellerLocations
            .Where(l => Haversine(req.Lat, req.Lng, l.Latitude!.Value, l.Longitude!.Value) <= req.RadiusKm)
            .Select(l => l.OwnerId)
            .ToList();

        if (sellerIds.Count == 0)
        {
            await SendAsync([], 200, ct);
            return;
        }

        var sellers = await db.Sellers
            .AsNoTracking()
            .Where(s => sellerIds.Contains(s.Id) && s.Status == SellerStatus.Active)
            .ToListAsync(ct);

        var result = sellers.Select(s =>
        {
            var loc = sellerLocations.First(l => l.OwnerId == s.Id);
            var dist = Haversine(req.Lat, req.Lng, loc.Latitude!.Value, loc.Longitude!.Value);
            return new NearbySeller(s.Id, s.UserId, s.BusinessName, s.Status, s.AverageRating, s.ReviewCount, Math.Round(dist, 2), loc.Latitude!.Value, loc.Longitude!.Value, s.CreatedAt);
        })
        .OrderBy(s => s.DistanceKm)
        .ToList();

        await SendAsync(result, 200, ct);
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180.0;
}

public class SuspendSellerEndpoint(LivestockDbContext db) : Endpoint<SuspendSellerRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Sellers/{Id}/Suspend");
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

public class DeleteSellerEndpoint(LivestockDbContext db) : Endpoint<DeleteSellerRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Sellers/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Sellers");
    }

    public override async Task HandleAsync(DeleteSellerRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == req.Id, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        seller.IsDeleted = true;
        seller.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
