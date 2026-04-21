using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.ShippingRates;

public class GetShippingRatesByZoneEndpoint(LivestockDbContext db) : Endpoint<GetShippingRatesByZoneRequest, List<ShippingRateItem>>
{
    public override void Configure()
    {
        Get("/Admin/ShippingZones/{ShippingZoneId}/Rates");
        AllowAnonymous();
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(GetShippingRatesByZoneRequest req, CancellationToken ct)
    {
        var rates = await db.ShippingRates
            .AsNoTracking()
            .Where(r => r.ShippingZoneId == req.ShippingZoneId && !r.IsDeleted)
            .OrderByDescending(r => r.IsActive)
            .Select(r => new ShippingRateItem(r.Id, r.ShippingZoneId, r.ShippingCarrierId, r.MinWeight, r.MaxWeight, r.MinOrderAmount, r.ShippingCost, r.Currency, r.EstimatedDeliveryDays, r.IsFreeShipping, r.IsActive, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(rates, 200, ct);
    }
}

public class GetShippingRateEndpoint(LivestockDbContext db) : Endpoint<GetShippingRateRequest, ShippingRateItem>
{
    public override void Configure()
    {
        Get("/Admin/ShippingRates/{Id}");
        AllowAnonymous();
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(GetShippingRateRequest req, CancellationToken ct)
    {
        var r = await db.ShippingRates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (r is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingRateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ShippingRateItem(r.Id, r.ShippingZoneId, r.ShippingCarrierId, r.MinWeight, r.MaxWeight, r.MinOrderAmount, r.ShippingCost, r.Currency, r.EstimatedDeliveryDays, r.IsFreeShipping, r.IsActive, r.CreatedAt), 200, ct);
    }
}

public class CreateShippingRateEndpoint(LivestockDbContext db) : Endpoint<CreateShippingRateRequest, ShippingRateItem>
{
    public override void Configure()
    {
        Post("/Admin/ShippingRates");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(CreateShippingRateRequest req, CancellationToken ct)
    {
        var zone = await db.ShippingZones.AsNoTracking().FirstOrDefaultAsync(z => z.Id == req.ShippingZoneId && !z.IsDeleted, ct);
        if (zone is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var rate = new ShippingRate
        {
            ShippingZoneId = req.ShippingZoneId,
            ShippingCarrierId = req.ShippingCarrierId,
            MinWeight = req.MinWeight,
            MaxWeight = req.MaxWeight,
            MinOrderAmount = req.MinOrderAmount,
            ShippingCost = req.ShippingCost,
            Currency = req.Currency.ToUpperInvariant(),
            EstimatedDeliveryDays = req.EstimatedDeliveryDays,
            IsFreeShipping = req.IsFreeShipping,
            IsActive = req.IsActive
        };

        db.ShippingRates.Add(rate);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingRateItem(rate.Id, rate.ShippingZoneId, rate.ShippingCarrierId, rate.MinWeight, rate.MaxWeight, rate.MinOrderAmount, rate.ShippingCost, rate.Currency, rate.EstimatedDeliveryDays, rate.IsFreeShipping, rate.IsActive, rate.CreatedAt), 201, ct);
    }
}

public class UpdateShippingRateEndpoint(LivestockDbContext db) : Endpoint<UpdateShippingRateRequest, ShippingRateItem>
{
    public override void Configure()
    {
        Put("/Admin/ShippingRates/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(UpdateShippingRateRequest req, CancellationToken ct)
    {
        var rate = await db.ShippingRates.FirstOrDefaultAsync(r => r.Id == req.Id && !r.IsDeleted, ct);
        if (rate is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingRateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        rate.ShippingCarrierId = req.ShippingCarrierId;
        rate.MinWeight = req.MinWeight;
        rate.MaxWeight = req.MaxWeight;
        rate.MinOrderAmount = req.MinOrderAmount;
        rate.ShippingCost = req.ShippingCost;
        rate.Currency = req.Currency.ToUpperInvariant();
        rate.EstimatedDeliveryDays = req.EstimatedDeliveryDays;
        rate.IsFreeShipping = req.IsFreeShipping;
        rate.IsActive = req.IsActive;
        rate.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingRateItem(rate.Id, rate.ShippingZoneId, rate.ShippingCarrierId, rate.MinWeight, rate.MaxWeight, rate.MinOrderAmount, rate.ShippingCost, rate.Currency, rate.EstimatedDeliveryDays, rate.IsFreeShipping, rate.IsActive, rate.CreatedAt), 200, ct);
    }
}

public class DeleteShippingRateEndpoint(LivestockDbContext db) : Endpoint<DeleteShippingRateRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/ShippingRates/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(DeleteShippingRateRequest req, CancellationToken ct)
    {
        var rate = await db.ShippingRates.FirstOrDefaultAsync(r => r.Id == req.Id && !r.IsDeleted, ct);
        if (rate is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingRateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        rate.IsDeleted = true;
        rate.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
