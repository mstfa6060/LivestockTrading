using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Subscriptions;

public class GetSubscriptionPlansEndpoint(LivestockDbContext db) : Endpoint<GetSubscriptionPlansRequest, List<SubscriptionPlanItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Subscriptions/Plans");
        AllowAnonymous();
        Tags("Subscriptions");
    }

    public override async Task HandleAsync(GetSubscriptionPlansRequest req, CancellationToken ct)
    {
        var plans = await db.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive && p.TargetType == req.TargetType)
            .OrderBy(p => p.Tier).ThenBy(p => p.Period)
            .Select(p => new SubscriptionPlanItem(p.Id, p.Name, p.Description, p.Tier, p.TargetType, p.Period, p.Price, p.CurrencyCode, p.MaxListings, p.MaxPhotos, p.HasBoostDiscount, p.FreeBoostsPerMonth, p.IsActive))
            .ToListAsync(ct);

        await SendAsync(plans, 200, ct);
    }
}

public class GetMySubscriptionEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<MySubscriptionItem?>
{
    public override void Configure()
    {
        Post("/livestocktrading/Subscriptions/My");
        Tags("Subscriptions");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        Guid subscriberId;
        if (seller is not null)
        {
            subscriberId = seller.Id;
        }
        else
        {
            var transporter = await db.Transporters.AsNoTracking().FirstOrDefaultAsync(t => t.UserId == user.UserId, ct);
            if (transporter is null) { await SendAsync(null, 200, ct); return; }
            subscriberId = transporter.Id;
        }

        var sub = await db.SellerSubscriptions
            .AsNoTracking()
            .Include(s => s.Plan)
            .Where(s => s.SubscriberId == subscriberId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.ExpiresAt)
            .FirstOrDefaultAsync(ct);

        if (sub is null) { await SendAsync(null, 200, ct); return; }

        await SendAsync(new MySubscriptionItem(sub.Id, sub.PlanId, sub.Plan.Name, sub.Plan.Tier, sub.Status, sub.Period, sub.StartedAt, sub.ExpiresAt, sub.PaidAmount, sub.CurrencyCode), 200, ct);
    }
}

public class SubscribeEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<SubscribeRequest, MySubscriptionItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/Subscriptions/Subscribe");
        Roles("LivestockTrading.Seller", "LivestockTrading.Transporter");
        Tags("Subscriptions");
    }

    public override async Task HandleAsync(SubscribeRequest req, CancellationToken ct)
    {
        var plan = await db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == req.PlanId && p.IsActive, ct);
        if (plan is null)
        {
            AddError(LivestockErrors.SubscriptionErrors.SubscriptionPlanNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        Guid subscriberId;
        if (plan.TargetType == SubscriptionTargetType.Seller)
        {
            var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
            if (seller is null) { AddError(LivestockErrors.SellerErrors.SellerNotFound); await SendErrorsAsync(403, ct); return; }
            subscriberId = seller.Id;
        }
        else
        {
            var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.UserId == user.UserId, ct);
            if (transporter is null) { AddError(LivestockErrors.TransportErrors.TransporterNotFound); await SendErrorsAsync(403, ct); return; }
            subscriberId = transporter.Id;
        }

        var hasActive = await db.SellerSubscriptions
            .AnyAsync(s => s.SubscriberId == subscriberId
                        && s.Status == SubscriptionStatus.Active
                        && s.ExpiresAt > DateTime.UtcNow, ct);
        if (hasActive)
        {
            AddError(LivestockErrors.SubscriptionErrors.AlreadyHasActiveSubscription);
            await SendErrorsAsync(409, ct);
            return;
        }

        var duplicateTx = await db.IAPTransactions
            .AnyAsync(t => t.Platform == req.Platform
                        && t.ExternalTransactionId == req.StoreTransactionId, ct);
        if (duplicateTx)
        {
            AddError(LivestockErrors.SubscriptionErrors.AlreadyHasActiveSubscription);
            await SendErrorsAsync(409, ct);
            return;
        }

        var expiresAt = plan.Period == SubscriptionPeriod.Monthly
            ? DateTime.UtcNow.AddMonths(1)
            : DateTime.UtcNow.AddYears(1);

        var subscription = new SellerSubscription
        {
            SubscriberId = subscriberId,
            SubscriberType = plan.TargetType,
            PlanId = plan.Id,
            Status = SubscriptionStatus.Active,
            Period = plan.Period,
            Platform = req.Platform,
            ExternalSubscriptionId = req.ExternalSubscriptionId,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            PaidAmount = plan.Price,
            CurrencyCode = plan.CurrencyCode
        };

        db.SellerSubscriptions.Add(subscription);

        db.IAPTransactions.Add(new IAPTransaction
        {
            UserId = user.UserId,
            TransactionType = IAPTransactionType.Subscription,
            Status = IAPTransactionStatus.Pending,
            Platform = req.Platform,
            ExternalTransactionId = req.StoreTransactionId,
            ProductId = req.ExternalSubscriptionId,
            ReceiptData = req.Receipt,
            Amount = plan.Price,
            CurrencyCode = plan.CurrencyCode,
            SubscriptionId = subscription.Id
        });

        await db.SaveChangesAsync(ct);

        await SendAsync(new MySubscriptionItem(subscription.Id, plan.Id, plan.Name, plan.Tier, subscription.Status, subscription.Period, subscription.StartedAt, subscription.ExpiresAt, subscription.PaidAmount, subscription.CurrencyCode), 201, ct);
    }
}

public class GetBoostPackagesEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<BoostPackageItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Boosts/Packages");
        AllowAnonymous();
        Tags("Subscriptions");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var packages = await db.BoostPackages
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.BoostType).ThenBy(p => p.Price)
            .Select(p => new BoostPackageItem(p.Id, p.Name, p.Description, p.BoostType, p.DurationDays, p.Price, p.CurrencyCode, p.IsActive))
            .ToListAsync(ct);

        await SendAsync(packages, 200, ct);
    }
}

public class PurchaseBoostEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<PurchaseBoostRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Boosts/Purchase");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Subscriptions");
    }

    public override async Task HandleAsync(PurchaseBoostRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null) { AddError(LivestockErrors.SellerErrors.SellerNotFound); await SendErrorsAsync(403, ct); return; }

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && p.SellerId == seller.Id, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }

        var package = await db.BoostPackages.FirstOrDefaultAsync(p => p.Id == req.PackageId && p.IsActive, ct);
        if (package is null) { AddError(LivestockErrors.SubscriptionErrors.BoostPackageNotFound); await SendErrorsAsync(404, ct); return; }

        var boost = new ProductBoost
        {
            ProductId = req.ProductId,
            SellerId = seller.Id,
            PackageId = req.PackageId,
            BoostType = package.BoostType,
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddDays(package.DurationDays),
            IsActive = true,
            PaidAmount = package.Price,
            CurrencyCode = package.CurrencyCode
        };

        db.ProductBoosts.Add(boost);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
