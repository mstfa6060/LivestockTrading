using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.BoostPackages;

public class ListBoostPackagesAdminEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<BoostPackageAdminItem>>
{
    public override void Configure()
    {
        Get("/Admin/BoostPackages");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var packages = await db.BoostPackages
            .AsNoTracking()
            .OrderBy(p => p.BoostType).ThenBy(p => p.Price)
            .Select(p => new BoostPackageAdminItem(p.Id, p.Name, p.Description, p.BoostType, p.DurationDays, p.Price, p.CurrencyCode, p.AppStoreProductId, p.PlayStoreProductId, p.MultiplierFactor, p.IsActive, p.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(packages, 200, ct);
    }
}

public class GetBoostPackageAdminEndpoint(LivestockDbContext db) : Endpoint<GetBoostPackageAdminRequest, BoostPackageAdminItem>
{
    public override void Configure()
    {
        Get("/Admin/BoostPackages/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(GetBoostPackageAdminRequest req, CancellationToken ct)
    {
        var p = await db.BoostPackages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (p is null)
        {
            AddError(LivestockErrors.SubscriptionErrors.BoostPackageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new BoostPackageAdminItem(p.Id, p.Name, p.Description, p.BoostType, p.DurationDays, p.Price, p.CurrencyCode, p.AppStoreProductId, p.PlayStoreProductId, p.MultiplierFactor, p.IsActive, p.CreatedAt), 200, ct);
    }
}

public class CreateBoostPackageEndpoint(LivestockDbContext db) : Endpoint<CreateBoostPackageRequest, BoostPackageAdminItem>
{
    public override void Configure()
    {
        Post("/Admin/BoostPackages");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CreateBoostPackageRequest req, CancellationToken ct)
    {
        var package = new BoostPackage
        {
            Name = req.Name,
            Description = req.Description,
            BoostType = req.BoostType,
            DurationDays = req.DurationDays,
            Price = req.Price,
            CurrencyCode = req.CurrencyCode,
            AppStoreProductId = req.AppStoreProductId,
            PlayStoreProductId = req.PlayStoreProductId,
            MultiplierFactor = req.MultiplierFactor
        };

        db.BoostPackages.Add(package);
        await db.SaveChangesAsync(ct);

        await SendAsync(new BoostPackageAdminItem(package.Id, package.Name, package.Description, package.BoostType, package.DurationDays, package.Price, package.CurrencyCode, package.AppStoreProductId, package.PlayStoreProductId, package.MultiplierFactor, package.IsActive, package.CreatedAt), 201, ct);
    }
}

public class UpdateBoostPackageEndpoint(LivestockDbContext db) : Endpoint<UpdateBoostPackageRequest, BoostPackageAdminItem>
{
    public override void Configure()
    {
        Put("/Admin/BoostPackages/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(UpdateBoostPackageRequest req, CancellationToken ct)
    {
        var package = await db.BoostPackages.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (package is null)
        {
            AddError(LivestockErrors.SubscriptionErrors.BoostPackageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        package.Name = req.Name;
        package.Description = req.Description;
        package.DurationDays = req.DurationDays;
        package.Price = req.Price;
        package.CurrencyCode = req.CurrencyCode;
        package.AppStoreProductId = req.AppStoreProductId;
        package.PlayStoreProductId = req.PlayStoreProductId;
        package.MultiplierFactor = req.MultiplierFactor;
        package.IsActive = req.IsActive;
        package.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new BoostPackageAdminItem(package.Id, package.Name, package.Description, package.BoostType, package.DurationDays, package.Price, package.CurrencyCode, package.AppStoreProductId, package.PlayStoreProductId, package.MultiplierFactor, package.IsActive, package.CreatedAt), 200, ct);
    }
}

public class DeleteBoostPackageEndpoint(LivestockDbContext db) : Endpoint<DeleteBoostPackageRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/BoostPackages/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(DeleteBoostPackageRequest req, CancellationToken ct)
    {
        var package = await db.BoostPackages.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (package is null)
        {
            AddError(LivestockErrors.SubscriptionErrors.BoostPackageNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        db.BoostPackages.Remove(package);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
