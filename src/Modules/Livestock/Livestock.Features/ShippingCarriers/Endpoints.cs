using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.ShippingCarriers;

public class GetAllCarriersEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<ShippingCarrierItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingCarriers/All");
        AllowAnonymous();
        Tags("Shipping");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var carriers = await db.ShippingCarriers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ShippingCarrierItem(
                c.Id, c.Name, c.Code, c.Website, c.TrackingUrlTemplate,
                c.IsActive, c.SupportedCountries, c.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(carriers, 200, ct);
    }
}

public class GetCarrierEndpoint(LivestockDbContext db) : Endpoint<GetCarrierRequest, ShippingCarrierItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingCarriers/Detail");
        AllowAnonymous();
        Tags("Shipping");
    }

    public override async Task HandleAsync(GetCarrierRequest req, CancellationToken ct)
    {
        var c = await db.ShippingCarriers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (c is null)
        {
            AddError(LivestockErrors.ShippingErrors.CarrierNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new ShippingCarrierItem(
            c.Id, c.Name, c.Code, c.Website, c.TrackingUrlTemplate,
            c.IsActive, c.SupportedCountries, c.CreatedAt), 200, ct);
    }
}

public class CreateCarrierEndpoint(LivestockDbContext db) : Endpoint<CreateCarrierRequest, ShippingCarrierItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingCarriers/Create");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(CreateCarrierRequest req, CancellationToken ct)
    {
        var codeExists = await db.ShippingCarriers.AnyAsync(c => c.Code == req.Code, ct);
        if (codeExists)
        {
            AddError(LivestockErrors.ShippingErrors.CarrierCodeAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var carrier = new ShippingCarrier
        {
            Name = req.Name,
            Code = req.Code,
            Website = req.Website,
            TrackingUrlTemplate = req.TrackingUrlTemplate,
            IsActive = req.IsActive,
            SupportedCountries = req.SupportedCountries,
        };

        db.ShippingCarriers.Add(carrier);
        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingCarrierItem(
            carrier.Id, carrier.Name, carrier.Code, carrier.Website,
            carrier.TrackingUrlTemplate, carrier.IsActive, carrier.SupportedCountries,
            carrier.CreatedAt), 201, ct);
    }
}

public class UpdateCarrierEndpoint(LivestockDbContext db) : Endpoint<UpdateCarrierRequest, ShippingCarrierItem>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingCarriers/Update");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(UpdateCarrierRequest req, CancellationToken ct)
    {
        var carrier = await db.ShippingCarriers.FirstOrDefaultAsync(c => c.Id == req.Id, ct);
        if (carrier is null)
        {
            AddError(LivestockErrors.ShippingErrors.CarrierNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (carrier.Code != req.Code)
        {
            var codeTaken = await db.ShippingCarriers
                .AnyAsync(c => c.Code == req.Code && c.Id != req.Id, ct);
            if (codeTaken)
            {
                AddError(LivestockErrors.ShippingErrors.CarrierCodeAlreadyExists);
                await SendErrorsAsync(409, ct);
                return;
            }
        }

        carrier.Name = req.Name;
        carrier.Code = req.Code;
        carrier.Website = req.Website;
        carrier.TrackingUrlTemplate = req.TrackingUrlTemplate;
        carrier.IsActive = req.IsActive;
        carrier.SupportedCountries = req.SupportedCountries;
        carrier.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        await SendAsync(new ShippingCarrierItem(
            carrier.Id, carrier.Name, carrier.Code, carrier.Website,
            carrier.TrackingUrlTemplate, carrier.IsActive, carrier.SupportedCountries,
            carrier.CreatedAt), 200, ct);
    }
}

public class DeleteCarrierEndpoint(LivestockDbContext db) : Endpoint<DeleteCarrierRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/ShippingCarriers/Delete");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Shipping");
    }

    public override async Task HandleAsync(DeleteCarrierRequest req, CancellationToken ct)
    {
        var carrier = await db.ShippingCarriers.FirstOrDefaultAsync(c => c.Id == req.Id, ct);
        if (carrier is null)
        {
            AddError(LivestockErrors.ShippingErrors.CarrierNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        carrier.IsDeleted = true;
        carrier.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
