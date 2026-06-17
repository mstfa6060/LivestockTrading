using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.ShippingZones;

public class GetAllZonesEndpoint(LivestockDbContext db) : Endpoint<GetZonesRequest, List<ShippingZoneItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingZones/All");
        AllowAnonymous();
        Tags("Shipping");
    }

    public override async Task HandleAsync(GetZonesRequest req, CancellationToken ct)
    {
        var query = db.ShippingZones.AsNoTracking().AsQueryable();
        if (req.SellerId is not null)
        {
            query = query.Where(z => z.SellerId == req.SellerId);
        }

        var zones = await query
            .OrderBy(z => z.Name)
            .Select(z => new ShippingZoneItem(
                z.Id, z.SellerId, z.Name, z.CountryCodes, z.IsActive, z.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(zones, 200, ct);
    }
}

public class GetZoneEndpoint(LivestockDbContext db) : Endpoint<GetZoneRequest, ShippingZoneItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingZones/Detail");
        AllowAnonymous();
        Tags("Shipping");
    }

    public override async Task HandleAsync(GetZoneRequest req, CancellationToken ct)
    {
        var z = await db.ShippingZones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (z is null)
        {
            AddError(LivestockErrors.ShippingErrors.ZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ShippingZoneItem(
            z.Id, z.SellerId, z.Name, z.CountryCodes, z.IsActive, z.CreatedAt), 200, ct);
    }
}

public class CreateZoneEndpoint(LivestockDbContext db) : Endpoint<CreateZoneRequest, ShippingZoneItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingZones/Create");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(CreateZoneRequest req, CancellationToken ct)
    {
        var zone = new ShippingZone
        {
            SellerId = req.SellerId,
            Name = req.Name,
            CountryCodes = req.CountryCodes,
            IsActive = req.IsActive,
        };

        db.ShippingZones.Add(zone);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingZoneItem(
            zone.Id, zone.SellerId, zone.Name, zone.CountryCodes, zone.IsActive, zone.CreatedAt), 201, ct);
    }
}

public class UpdateZoneEndpoint(LivestockDbContext db) : Endpoint<UpdateZoneRequest, ShippingZoneItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingZones/Update");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(UpdateZoneRequest req, CancellationToken ct)
    {
        var zone = await db.ShippingZones.FirstOrDefaultAsync(z => z.Id == req.Id, ct);
        if (zone is null)
        {
            AddError(LivestockErrors.ShippingErrors.ZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        zone.SellerId = req.SellerId;
        zone.Name = req.Name;
        zone.CountryCodes = req.CountryCodes;
        zone.IsActive = req.IsActive;
        zone.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingZoneItem(
            zone.Id, zone.SellerId, zone.Name, zone.CountryCodes, zone.IsActive, zone.CreatedAt), 200, ct);
    }
}

public class DeleteZoneEndpoint(LivestockDbContext db) : Endpoint<DeleteZoneRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingZones/Delete");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(DeleteZoneRequest req, CancellationToken ct)
    {
        var zone = await db.ShippingZones.FirstOrDefaultAsync(z => z.Id == req.Id, ct);
        if (zone is null)
        {
            AddError(LivestockErrors.ShippingErrors.ZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        zone.IsDeleted = true;
        zone.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
