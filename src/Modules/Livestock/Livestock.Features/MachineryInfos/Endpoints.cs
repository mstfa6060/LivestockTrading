using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.MachineryInfos;

public class GetMachineryInfoByProductEndpoint(LivestockDbContext db) : Endpoint<GetMachineryInfoByProductRequest, MachineryInfoDetail>
{
    public override void Configure()
    {
        Get("/MachineryInfos/ByProduct/{ProductId}");
        AllowAnonymous();
        Tags("MachineryInfos");
    }

    public override async Task HandleAsync(GetMachineryInfoByProductRequest req, CancellationToken ct)
    {
        var m = await db.MachineryInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (m is null) { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(MachineryInfoMapper.Map(m), 200, ct);
    }
}

public class GetMachineryInfoEndpoint(LivestockDbContext db) : Endpoint<GetMachineryInfoRequest, MachineryInfoDetail>
{
    public override void Configure()
    {
        Get("/MachineryInfos/{Id}");
        AllowAnonymous();
        Tags("MachineryInfos");
    }

    public override async Task HandleAsync(GetMachineryInfoRequest req, CancellationToken ct)
    {
        var m = await db.MachineryInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null) { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(MachineryInfoMapper.Map(m), 200, ct);
    }
}

public class CreateMachineryInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateMachineryInfoRequest, MachineryInfoDetail>
{
    public override void Configure()
    {
        Post("/MachineryInfos");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("MachineryInfos");
    }

    public override async Task HandleAsync(CreateMachineryInfoRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && !p.IsDeleted, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var exists = await db.MachineryInfos.AnyAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (exists) { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoAlreadyExists); await SendErrorsAsync(409, ct); return; }

        var m = new MachineryInfo
        {
            ProductId = req.ProductId, MachineryType = req.MachineryType, Manufacturer = req.Manufacturer,
            Model = req.Model, Year = req.Year, HorsePower = req.HorsePower, WorkingHours = req.WorkingHours,
            FuelType = req.FuelType, Dimensions = req.Dimensions, WeightKg = req.WeightKg,
            HasWarranty = req.HasWarranty, WarrantyExpiresAt = req.WarrantyExpiresAt,
            SerialNumber = req.SerialNumber, AdditionalFeatures = req.AdditionalFeatures
        };
        db.MachineryInfos.Add(m);
        await db.SaveChangesAsync(ct);
        await SendAsync(MachineryInfoMapper.Map(m), 201, ct);
    }
}

public class UpdateMachineryInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateMachineryInfoRequest, MachineryInfoDetail>
{
    public override void Configure()
    {
        Put("/MachineryInfos/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("MachineryInfos");
    }

    public override async Task HandleAsync(UpdateMachineryInfoRequest req, CancellationToken ct)
    {
        var m = await db.MachineryInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null) { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || m.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        m.MachineryType = req.MachineryType; m.Manufacturer = req.Manufacturer; m.Model = req.Model;
        m.Year = req.Year; m.HorsePower = req.HorsePower; m.WorkingHours = req.WorkingHours;
        m.FuelType = req.FuelType; m.Dimensions = req.Dimensions; m.WeightKg = req.WeightKg;
        m.HasWarranty = req.HasWarranty; m.WarrantyExpiresAt = req.WarrantyExpiresAt;
        m.SerialNumber = req.SerialNumber; m.AdditionalFeatures = req.AdditionalFeatures;
        m.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(MachineryInfoMapper.Map(m), 200, ct);
    }
}

public class DeleteMachineryInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteMachineryInfoRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/MachineryInfos/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("MachineryInfos");
    }

    public override async Task HandleAsync(DeleteMachineryInfoRequest req, CancellationToken ct)
    {
        var m = await db.MachineryInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null) { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || m.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.MachineryInfoErrors.MachineryInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        m.IsDeleted = true; m.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class MachineryInfoMapper
{
    internal static MachineryInfoDetail Map(MachineryInfo m) => new(
        m.Id, m.ProductId, m.MachineryType, m.Manufacturer, m.Model, m.Year,
        m.HorsePower, m.WorkingHours, m.FuelType, m.Dimensions, m.WeightKg,
        m.HasWarranty, m.WarrantyExpiresAt, m.SerialNumber, m.AdditionalFeatures, m.CreatedAt);
}
