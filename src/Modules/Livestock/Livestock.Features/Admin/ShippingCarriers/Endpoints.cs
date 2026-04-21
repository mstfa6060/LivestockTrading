using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.ShippingCarriers;

public class ListShippingCarriersEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<ShippingCarrierItem>>
{
    public override void Configure()
    {
        Get("/Admin/ShippingCarriers");
        AllowAnonymous();
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var carriers = await db.ShippingCarriers
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new ShippingCarrierItem(c.Id, c.Name, c.Code, c.Website, c.TrackingUrlTemplate, c.IsActive, c.SupportedCountries, c.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(carriers, 200, ct);
    }
}

public class GetShippingCarrierEndpoint(LivestockDbContext db) : Endpoint<GetShippingCarrierRequest, ShippingCarrierItem>
{
    public override void Configure()
    {
        Get("/Admin/ShippingCarriers/{Id}");
        AllowAnonymous();
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(GetShippingCarrierRequest req, CancellationToken ct)
    {
        var c = await db.ShippingCarriers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (c is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingCarrierNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ShippingCarrierItem(c.Id, c.Name, c.Code, c.Website, c.TrackingUrlTemplate, c.IsActive, c.SupportedCountries, c.CreatedAt), 200, ct);
    }
}

public class CreateShippingCarrierEndpoint(LivestockDbContext db) : Endpoint<CreateShippingCarrierRequest, ShippingCarrierItem>
{
    public override void Configure()
    {
        Post("/Admin/ShippingCarriers");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(CreateShippingCarrierRequest req, CancellationToken ct)
    {
        var exists = await db.ShippingCarriers.AnyAsync(c => c.Code == req.Code && !c.IsDeleted, ct);
        if (exists)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingCarrierCodeAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var carrier = new ShippingCarrier
        {
            Name = req.Name,
            Code = req.Code.ToUpperInvariant(),
            Website = req.Website,
            TrackingUrlTemplate = req.TrackingUrlTemplate,
            IsActive = req.IsActive,
            SupportedCountries = req.SupportedCountries
        };

        db.ShippingCarriers.Add(carrier);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingCarrierItem(carrier.Id, carrier.Name, carrier.Code, carrier.Website, carrier.TrackingUrlTemplate, carrier.IsActive, carrier.SupportedCountries, carrier.CreatedAt), 201, ct);
    }
}

public class UpdateShippingCarrierEndpoint(LivestockDbContext db) : Endpoint<UpdateShippingCarrierRequest, ShippingCarrierItem>
{
    public override void Configure()
    {
        Put("/Admin/ShippingCarriers/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(UpdateShippingCarrierRequest req, CancellationToken ct)
    {
        var carrier = await db.ShippingCarriers.FirstOrDefaultAsync(c => c.Id == req.Id && !c.IsDeleted, ct);
        if (carrier is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingCarrierNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        carrier.Name = req.Name;
        carrier.Website = req.Website;
        carrier.TrackingUrlTemplate = req.TrackingUrlTemplate;
        carrier.IsActive = req.IsActive;
        carrier.SupportedCountries = req.SupportedCountries;
        carrier.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingCarrierItem(carrier.Id, carrier.Name, carrier.Code, carrier.Website, carrier.TrackingUrlTemplate, carrier.IsActive, carrier.SupportedCountries, carrier.CreatedAt), 200, ct);
    }
}

public class DeleteShippingCarrierEndpoint(LivestockDbContext db) : Endpoint<DeleteShippingCarrierRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/ShippingCarriers/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin", "Shipping");
    }

    public override async Task HandleAsync(DeleteShippingCarrierRequest req, CancellationToken ct)
    {
        var carrier = await db.ShippingCarriers.FirstOrDefaultAsync(c => c.Id == req.Id && !c.IsDeleted, ct);
        if (carrier is null)
        {
            AddError(LivestockErrors.ShippingErrors.ShippingCarrierNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        carrier.IsDeleted = true;
        carrier.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
