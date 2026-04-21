using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.ShippingZones;

public class ListShippingZonesEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<ShippingZoneItem>>
{
    public override void Configure()
    {
        Get("/Admin/ShippingZones");
        AllowAnonymous();
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var zones = await db.ShippingZones
            .AsNoTracking()
            .Where(z => !z.IsDeleted)
            .OrderBy(z => z.Name)
            .Select(z => new ShippingZoneItem(z.Id, z.SellerId, z.Name, z.CountryCodes, z.IsActive, z.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(zones, 200, ct);
    }
}

public class GetShippingZoneEndpoint(LivestockDbContext db) : Endpoint<GetShippingZoneRequest, ShippingZoneItem>
{
    public override void Configure()
    {
        Get("/Admin/ShippingZones/{Id}");
        AllowAnonymous();
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(GetShippingZoneRequest req, CancellationToken ct)
    {
        var z = await db.ShippingZones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (z is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ShippingZoneItem(z.Id, z.SellerId, z.Name, z.CountryCodes, z.IsActive, z.CreatedAt), 200, ct);
    }
}

public class CreateShippingZoneEndpoint(LivestockDbContext db) : Endpoint<CreateShippingZoneRequest, ShippingZoneItem>
{
    public override void Configure()
    {
        Post("/Admin/ShippingZones");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(CreateShippingZoneRequest req, CancellationToken ct)
    {
        var zone = new ShippingZone
        {
            Name = req.Name,
            SellerId = req.SellerId,
            CountryCodes = req.CountryCodes,
            IsActive = req.IsActive
        };

        db.ShippingZones.Add(zone);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingZoneItem(zone.Id, zone.SellerId, zone.Name, zone.CountryCodes, zone.IsActive, zone.CreatedAt), 201, ct);
    }
}

public class UpdateShippingZoneEndpoint(LivestockDbContext db) : Endpoint<UpdateShippingZoneRequest, ShippingZoneItem>
{
    public override void Configure()
    {
        Put("/Admin/ShippingZones/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(UpdateShippingZoneRequest req, CancellationToken ct)
    {
        var zone = await db.ShippingZones.FirstOrDefaultAsync(z => z.Id == req.Id && !z.IsDeleted, ct);
        if (zone is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        zone.Name = req.Name;
        zone.SellerId = req.SellerId;
        zone.CountryCodes = req.CountryCodes;
        zone.IsActive = req.IsActive;
        zone.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingZoneItem(zone.Id, zone.SellerId, zone.Name, zone.CountryCodes, zone.IsActive, zone.CreatedAt), 200, ct);
    }
}

public class DeleteShippingZoneEndpoint(LivestockDbContext db) : Endpoint<DeleteShippingZoneRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/ShippingZones/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(DeleteShippingZoneRequest req, CancellationToken ct)
    {
        var zone = await db.ShippingZones.FirstOrDefaultAsync(z => z.Id == req.Id && !z.IsDeleted, ct);
        if (zone is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingZoneNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        zone.IsDeleted = true;
        zone.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
