using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.ShippingRates;

public class GetAllRatesEndpoint(LivestockDbContext db) : Endpoint<GetRatesRequest, List<ShippingRateItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingRates/All");
        AllowAnonymous();
        Tags("Shipping");
    }

    public override async Task HandleAsync(GetRatesRequest req, CancellationToken ct)
    {
        var rates = await db.ShippingRates
            .AsNoTracking()
            .Where(r => r.ShippingZoneId == req.ShippingZoneId)
            .OrderBy(r => r.MinWeight ?? 0)
            .Select(r => new ShippingRateItem(
                r.Id, r.ShippingZoneId, r.ShippingCarrierId,
                r.MinWeight, r.MaxWeight, r.MinOrderAmount,
                r.ShippingCost, r.CurrencyCode, r.EstimatedDeliveryDays,
                r.IsFreeShipping, r.IsActive, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(rates, 200, ct);
    }
}

public class GetRateEndpoint(LivestockDbContext db) : Endpoint<GetRateRequest, ShippingRateItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingRates/Detail");
        AllowAnonymous();
        Tags("Shipping");
    }

    public override async Task HandleAsync(GetRateRequest req, CancellationToken ct)
    {
        var r = await db.ShippingRates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (r is null)
        {
            AddError(LivestockErrors.ShippingErrors.RateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ShippingRateItem(
            r.Id, r.ShippingZoneId, r.ShippingCarrierId,
            r.MinWeight, r.MaxWeight, r.MinOrderAmount,
            r.ShippingCost, r.CurrencyCode, r.EstimatedDeliveryDays,
            r.IsFreeShipping, r.IsActive, r.CreatedAt), 200, ct);
    }
}

public class CreateRateEndpoint(LivestockDbContext db) : Endpoint<CreateRateRequest, ShippingRateItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingRates/Create");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(CreateRateRequest req, CancellationToken ct)
    {
        var zoneExists = await db.ShippingZones.AnyAsync(z => z.Id == req.ShippingZoneId, ct);
        if (!zoneExists)
        {
            AddError(LivestockErrors.ShippingErrors.ZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (req.ShippingCarrierId is { } carrierId)
        {
            var carrierExists = await db.ShippingCarriers.AnyAsync(c => c.Id == carrierId, ct);
            if (!carrierExists)
            {
                AddError(LivestockErrors.ShippingErrors.CarrierNotFound);
                await SendErrorsAsync(404, ct);
                return;
            }
        }

        var rate = new ShippingRate
        {
            ShippingZoneId = req.ShippingZoneId,
            ShippingCarrierId = req.ShippingCarrierId,
            MinWeight = req.MinWeight,
            MaxWeight = req.MaxWeight,
            MinOrderAmount = req.MinOrderAmount,
            ShippingCost = req.ShippingCost,
            CurrencyCode = req.CurrencyCode,
            EstimatedDeliveryDays = req.EstimatedDeliveryDays,
            IsFreeShipping = req.IsFreeShipping,
            IsActive = req.IsActive,
        };

        db.ShippingRates.Add(rate);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingRateItem(
            rate.Id, rate.ShippingZoneId, rate.ShippingCarrierId,
            rate.MinWeight, rate.MaxWeight, rate.MinOrderAmount,
            rate.ShippingCost, rate.CurrencyCode, rate.EstimatedDeliveryDays,
            rate.IsFreeShipping, rate.IsActive, rate.CreatedAt), 201, ct);
    }
}

public class UpdateRateEndpoint(LivestockDbContext db) : Endpoint<UpdateRateRequest, ShippingRateItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingRates/Update");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(UpdateRateRequest req, CancellationToken ct)
    {
        var rate = await db.ShippingRates.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (rate is null)
        {
            AddError(LivestockErrors.ShippingErrors.RateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var zoneExists = await db.ShippingZones.AnyAsync(z => z.Id == req.ShippingZoneId, ct);
        if (!zoneExists)
        {
            AddError(LivestockErrors.ShippingErrors.ZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (req.ShippingCarrierId is { } carrierId)
        {
            var carrierExists = await db.ShippingCarriers.AnyAsync(c => c.Id == carrierId, ct);
            if (!carrierExists)
            {
                AddError(LivestockErrors.ShippingErrors.CarrierNotFound);
                await SendErrorsAsync(404, ct);
                return;
            }
        }

        rate.ShippingZoneId = req.ShippingZoneId;
        rate.ShippingCarrierId = req.ShippingCarrierId;
        rate.MinWeight = req.MinWeight;
        rate.MaxWeight = req.MaxWeight;
        rate.MinOrderAmount = req.MinOrderAmount;
        rate.ShippingCost = req.ShippingCost;
        rate.CurrencyCode = req.CurrencyCode;
        rate.EstimatedDeliveryDays = req.EstimatedDeliveryDays;
        rate.IsFreeShipping = req.IsFreeShipping;
        rate.IsActive = req.IsActive;
        rate.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingRateItem(
            rate.Id, rate.ShippingZoneId, rate.ShippingCarrierId,
            rate.MinWeight, rate.MaxWeight, rate.MinOrderAmount,
            rate.ShippingCost, rate.CurrencyCode, rate.EstimatedDeliveryDays,
            rate.IsFreeShipping, rate.IsActive, rate.CreatedAt), 200, ct);
    }
}

public class DeleteRateEndpoint(LivestockDbContext db) : Endpoint<DeleteRateRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingRates/Delete");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(DeleteRateRequest req, CancellationToken ct)
    {
        var rate = await db.ShippingRates.FirstOrDefaultAsync(r => r.Id == req.Id, ct);
        if (rate is null)
        {
            AddError(LivestockErrors.ShippingErrors.RateNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        rate.IsDeleted = true;
        rate.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
