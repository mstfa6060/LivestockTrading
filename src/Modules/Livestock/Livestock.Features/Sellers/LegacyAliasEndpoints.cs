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

// Legacy-compat aliases so the frontend's generated client keeps working.
// The new canonical routes are /Sellers/Me (JWT-scoped) and /Sellers/Register
// (takes a trimmed BecomeSellerRequest); these wrap them with the shapes the
// old client expects:
//   - POST /Sellers/GetByUserId  { userId } -> full-SellerProfile
//   - POST /Sellers/Create       { userId, businessName, ... } -> same shape

public sealed record SellerLegacyRequest(Guid UserId);

public sealed record SellerLegacyProfile(
    Guid Id,
    Guid UserId,
    string BusinessName,
    string? BusinessType,
    string? TaxNumber,
    string? RegistrationNumber,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? Email,
    string? Phone,
    string? Website,
    bool IsVerified,
    DateTime? VerifiedAt,
    bool IsActive,
    int Status,
    double? AverageRating,
    int TotalReviews,
    int TotalSales,
    double TotalRevenue,
    string? BusinessHours,
    string? AcceptedPaymentMethods,
    string? ReturnPolicy,
    string? ShippingPolicy,
    string? SocialMediaLinks,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public sealed record SellerCreateLegacyRequest(
    Guid UserId,
    string BusinessName,
    string? BusinessType,
    string? TaxNumber,
    string? RegistrationNumber,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? Email,
    string? Phone,
    string? Website,
    bool IsActive,
    int Status,
    string? BusinessHours,
    string? AcceptedPaymentMethods,
    string? ReturnPolicy,
    string? ShippingPolicy,
    string? SocialMediaLinks
);

public sealed class SellersGetByUserIdEndpoint(LivestockDbContext db)
    : Endpoint<SellerLegacyRequest, SellerLegacyProfile>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/GetByUserId");
        Tags("Sellers");
    }

    public override async Task HandleAsync(SellerLegacyRequest req, CancellationToken ct)
    {
        var s = await db.Sellers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == req.UserId, ct);

        if (s is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(MapToLegacy(s), 200, ct);
    }

    internal static SellerLegacyProfile MapToLegacy(Seller s) => new(
        s.Id,
        s.UserId,
        s.BusinessName,
        BusinessType: null,
        s.TaxNumber,
        RegistrationNumber: null,
        s.Description,
        s.LogoUrl,
        BannerUrl: null,
        s.Email,
        s.PhoneNumber,
        s.WebsiteUrl,
        IsVerified: s.IsVerified,
        s.VerifiedAt,
        IsActive: s.IsActive,
        Status: (int)s.Status,
        AverageRating: s.AverageRating,
        TotalReviews: s.ReviewCount,
        TotalSales: 0,
        TotalRevenue: 0,
        BusinessHours: null,
        AcceptedPaymentMethods: null,
        ReturnPolicy: null,
        ShippingPolicy: null,
        SocialMediaLinks: null,
        s.CreatedAt,
        s.UpdatedAt);
}

public sealed class SellersCreateLegacyEndpoint(
    LivestockDbContext db,
    IUserContext user,
    IEventPublisher publisher)
    : Endpoint<SellerCreateLegacyRequest, SellerLegacyProfile>
{
    public override void Configure()
    {
        Post("/livestocktrading/Sellers/Create");
        Tags("Sellers");
    }

    public override async Task HandleAsync(SellerCreateLegacyRequest req, CancellationToken ct)
    {
        // Security: the new seller always belongs to the authenticated caller;
        // req.UserId is kept for payload parity with the old contract but not
        // trusted — otherwise any buyer could create a seller profile for an
        // arbitrary user id.
        var ownerId = user.UserId;

        var exists = await db.Sellers.AnyAsync(s => s.UserId == ownerId, ct);
        if (exists)
        {
            AddError(LivestockErrors.SellerErrors.SellerAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        // Match the auto-create path in Products/Create: the first seller
        // profile for a user is Active immediately. Forcing PendingVerification
        // here would block the listing-create flow on its very first call
        // ("SELLER_NOT_VERIFIED" 403) even though the Products endpoint
        // happily auto-activates when it has to create the seller itself.
        var seller = new Seller
        {
            UserId = ownerId,
            BusinessName = req.BusinessName,
            Description = req.Description,
            PhoneNumber = req.Phone,
            Email = req.Email,
            WebsiteUrl = req.Website,
            TaxNumber = req.TaxNumber,
            LogoUrl = req.LogoUrl,
            Status = SellerStatus.Active,
        };

        db.Sellers.Add(seller);
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(SellerRegisteredEvent.Subject, new SellerRegisteredEvent
        {
            SellerId = seller.Id,
            UserId = seller.UserId,
            BusinessName = seller.BusinessName,
        }, ct);

        await SendAsync(SellersGetByUserIdEndpoint.MapToLegacy(seller), 201, ct);
    }
}
