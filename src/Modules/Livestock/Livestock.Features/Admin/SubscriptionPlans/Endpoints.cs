using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.SubscriptionPlans;

public class ListSubscriptionPlansAdminEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<SubscriptionPlanAdminItem>>
{
    public override void Configure()
    {
        Get("/Admin/SubscriptionPlans");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var plans = await db.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(p => p.TargetType).ThenBy(p => p.Tier).ThenBy(p => p.Period)
            .Select(p => new SubscriptionPlanAdminItem(p.Id, p.Name, p.Description, p.Tier, p.TargetType, p.Period, p.Price, p.CurrencyCode, p.AppStoreProductId, p.PlayStoreProductId, p.MaxListings, p.MaxPhotos, p.HasBoostDiscount, p.FreeBoostsPerMonth, p.IsActive, p.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(plans, 200, ct);
    }
}

public class GetSubscriptionPlanAdminEndpoint(LivestockDbContext db) : Endpoint<GetSubscriptionPlanAdminRequest, SubscriptionPlanAdminItem>
{
    public override void Configure()
    {
        Get("/Admin/SubscriptionPlans/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(GetSubscriptionPlanAdminRequest req, CancellationToken ct)
    {
        var p = await db.SubscriptionPlans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (p is null)
        {
            AddError(LivestockErrors.SubscriptionErrors.SubscriptionPlanNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new SubscriptionPlanAdminItem(p.Id, p.Name, p.Description, p.Tier, p.TargetType, p.Period, p.Price, p.CurrencyCode, p.AppStoreProductId, p.PlayStoreProductId, p.MaxListings, p.MaxPhotos, p.HasBoostDiscount, p.FreeBoostsPerMonth, p.IsActive, p.CreatedAt), 200, ct);
    }
}

public class CreateSubscriptionPlanEndpoint(LivestockDbContext db) : Endpoint<CreateSubscriptionPlanRequest, SubscriptionPlanAdminItem>
{
    public override void Configure()
    {
        Post("/Admin/SubscriptionPlans");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CreateSubscriptionPlanRequest req, CancellationToken ct)
    {
        var plan = new SubscriptionPlan
        {
            Name = req.Name,
            Description = req.Description,
            Tier = req.Tier,
            TargetType = req.TargetType,
            Period = req.Period,
            Price = req.Price,
            CurrencyCode = req.CurrencyCode,
            AppStoreProductId = req.AppStoreProductId,
            PlayStoreProductId = req.PlayStoreProductId,
            MaxListings = req.MaxListings,
            MaxPhotos = req.MaxPhotos,
            HasBoostDiscount = req.HasBoostDiscount,
            FreeBoostsPerMonth = req.FreeBoostsPerMonth
        };

        db.SubscriptionPlans.Add(plan);
        await db.SaveChangesAsync(ct);

        await SendAsync(new SubscriptionPlanAdminItem(plan.Id, plan.Name, plan.Description, plan.Tier, plan.TargetType, plan.Period, plan.Price, plan.CurrencyCode, plan.AppStoreProductId, plan.PlayStoreProductId, plan.MaxListings, plan.MaxPhotos, plan.HasBoostDiscount, plan.FreeBoostsPerMonth, plan.IsActive, plan.CreatedAt), 201, ct);
    }
}

public class UpdateSubscriptionPlanEndpoint(LivestockDbContext db) : Endpoint<UpdateSubscriptionPlanRequest, SubscriptionPlanAdminItem>
{
    public override void Configure()
    {
        Put("/Admin/SubscriptionPlans/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(UpdateSubscriptionPlanRequest req, CancellationToken ct)
    {
        var plan = await db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (plan is null)
        {
            AddError(LivestockErrors.SubscriptionErrors.SubscriptionPlanNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        plan.Name = req.Name;
        plan.Description = req.Description;
        plan.Price = req.Price;
        plan.CurrencyCode = req.CurrencyCode;
        plan.AppStoreProductId = req.AppStoreProductId;
        plan.PlayStoreProductId = req.PlayStoreProductId;
        plan.MaxListings = req.MaxListings;
        plan.MaxPhotos = req.MaxPhotos;
        plan.HasBoostDiscount = req.HasBoostDiscount;
        plan.FreeBoostsPerMonth = req.FreeBoostsPerMonth;
        plan.IsActive = req.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new SubscriptionPlanAdminItem(plan.Id, plan.Name, plan.Description, plan.Tier, plan.TargetType, plan.Period, plan.Price, plan.CurrencyCode, plan.AppStoreProductId, plan.PlayStoreProductId, plan.MaxListings, plan.MaxPhotos, plan.HasBoostDiscount, plan.FreeBoostsPerMonth, plan.IsActive, plan.CreatedAt), 200, ct);
    }
}

public class DeleteSubscriptionPlanEndpoint(LivestockDbContext db) : Endpoint<DeleteSubscriptionPlanRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/SubscriptionPlans/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(DeleteSubscriptionPlanRequest req, CancellationToken ct)
    {
        var plan = await db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (plan is null)
        {
            AddError(LivestockErrors.SubscriptionErrors.SubscriptionPlanNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        db.SubscriptionPlans.Remove(plan);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
