using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Products.Approve;

public class ApproveProductEndpoint(LivestockDbContext db, IEventPublisher publisher) : Endpoint<ApproveProductRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Products/Approve");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Products");
    }

    public override async Task HandleAsync(ApproveProductRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (product is null)
        {
            AddError(LivestockErrors.ProductErrors.ProductNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var sellerLocation = await db.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.OwnerId == product.SellerId && l.OwnerType == "Seller", ct);

        product.Status = ProductStatus.Active;
        product.PublishedAt ??= DateTime.UtcNow;
        product.ExpiresAt = ComputeExpiry(sellerLocation?.CountryCode);
        product.RejectionReason = null;
        product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(ProductApprovedEvent.Subject, new ProductApprovedEvent
        {
            ProductId = product.Id,
            SellerId = product.SellerId
        }, ct);

        await SendNoContentAsync(ct);
    }

    private static DateTime ComputeExpiry(string? countryCode)
    {
        var tzId = GetTimezoneForCountry(countryCode);
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var nowInTz = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            var expiryDateInTz = nowInTz.Date.AddDays(30);
            var expiryInTz = new DateTime(expiryDateInTz.Year, expiryDateInTz.Month, expiryDateInTz.Day, 23, 59, 59, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(expiryInTz, tz);
        }
        catch
        {
            return DateTime.UtcNow.Date.AddDays(30).AddHours(23).AddMinutes(59).AddSeconds(59);
        }
    }

    private static string GetTimezoneForCountry(string? countryCode) => countryCode switch
    {
        "TR" => "Europe/Istanbul",
        "DE" => "Europe/Berlin",
        "FR" => "Europe/Paris",
        "GB" => "Europe/London",
        "RU" => "Europe/Moscow",
        "US" => "America/New_York",
        "CN" => "Asia/Shanghai",
        "JP" => "Asia/Tokyo",
        "AU" => "Australia/Sydney",
        "AE" => "Asia/Dubai",
        "SA" => "Asia/Riyadh",
        "KZ" => "Asia/Almaty",
        "PL" => "Europe/Warsaw",
        "UA" => "Europe/Kyiv",
        _ => "UTC"
    };
}
