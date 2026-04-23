using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.ChemicalInfos;

public class GetChemicalInfoByProductEndpoint(LivestockDbContext db) : Endpoint<GetChemicalInfoByProductRequest, ChemicalInfoDetail>
{
    public override void Configure()
    {
        Get("/ChemicalInfos/ByProduct/{ProductId}");
        AllowAnonymous();
        Tags("ChemicalInfos");
    }

    public override async Task HandleAsync(GetChemicalInfoByProductRequest req, CancellationToken ct)
    {
        var c = await db.ChemicalInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (c is null) { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(ChemicalInfoMapper.Map(c), 200, ct);
    }
}

public class GetChemicalInfoEndpoint(LivestockDbContext db) : Endpoint<GetChemicalInfoRequest, ChemicalInfoDetail>
{
    public override void Configure()
    {
        Get("/ChemicalInfos/{Id}");
        AllowAnonymous();
        Tags("ChemicalInfos");
    }

    public override async Task HandleAsync(GetChemicalInfoRequest req, CancellationToken ct)
    {
        var c = await db.ChemicalInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (c is null) { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(ChemicalInfoMapper.Map(c), 200, ct);
    }
}

public class CreateChemicalInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateChemicalInfoRequest, ChemicalInfoDetail>
{
    public override void Configure()
    {
        Post("/ChemicalInfos");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ChemicalInfos");
    }

    public override async Task HandleAsync(CreateChemicalInfoRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && !p.IsDeleted, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var exists = await db.ChemicalInfos.AnyAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (exists) { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoAlreadyExists); await SendErrorsAsync(409, ct); return; }

        var c = new ChemicalInfo
        {
            ProductId = req.ProductId, ChemicalType = req.ChemicalType, ToxicityLevel = req.ToxicityLevel,
            ActiveIngredient = req.ActiveIngredient, Concentration = req.Concentration,
            TargetPest = req.TargetPest, TargetCrop = req.TargetCrop,
            ApplicationMethod = req.ApplicationMethod, SafetyPrecautions = req.SafetyPrecautions,
            StorageConditions = req.StorageConditions, WithholdingPeriodDays = req.WithholdingPeriodDays,
            RegistrationNumber = req.RegistrationNumber, ExpiryDate = req.ExpiryDate,
            VolumeOrWeightPerUnit = req.VolumeOrWeightPerUnit, Unit = req.Unit
        };
        db.ChemicalInfos.Add(c);
        await db.SaveChangesAsync(ct);
        await SendAsync(ChemicalInfoMapper.Map(c), 201, ct);
    }
}

public class UpdateChemicalInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateChemicalInfoRequest, ChemicalInfoDetail>
{
    public override void Configure()
    {
        Put("/ChemicalInfos/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ChemicalInfos");
    }

    public override async Task HandleAsync(UpdateChemicalInfoRequest req, CancellationToken ct)
    {
        var c = await db.ChemicalInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (c is null) { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || c.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        c.ChemicalType = req.ChemicalType; c.ToxicityLevel = req.ToxicityLevel;
        c.ActiveIngredient = req.ActiveIngredient; c.Concentration = req.Concentration;
        c.TargetPest = req.TargetPest; c.TargetCrop = req.TargetCrop;
        c.ApplicationMethod = req.ApplicationMethod; c.SafetyPrecautions = req.SafetyPrecautions;
        c.StorageConditions = req.StorageConditions; c.WithholdingPeriodDays = req.WithholdingPeriodDays;
        c.RegistrationNumber = req.RegistrationNumber; c.ExpiryDate = req.ExpiryDate;
        c.VolumeOrWeightPerUnit = req.VolumeOrWeightPerUnit; c.Unit = req.Unit;
        c.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(ChemicalInfoMapper.Map(c), 200, ct);
    }
}

public class DeleteChemicalInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteChemicalInfoRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/ChemicalInfos/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("ChemicalInfos");
    }

    public override async Task HandleAsync(DeleteChemicalInfoRequest req, CancellationToken ct)
    {
        var c = await db.ChemicalInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (c is null) { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || c.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.ChemicalInfoErrors.ChemicalInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        c.IsDeleted = true; c.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class ChemicalInfoMapper
{
    internal static ChemicalInfoDetail Map(ChemicalInfo c) => new(
        c.Id, c.ProductId, c.ChemicalType, c.ToxicityLevel, c.ActiveIngredient, c.Concentration,
        c.TargetPest, c.TargetCrop, c.ApplicationMethod, c.SafetyPrecautions,
        c.StorageConditions, c.WithholdingPeriodDays, c.RegistrationNumber, c.ExpiryDate,
        c.VolumeOrWeightPerUnit, c.Unit, c.CreatedAt);
}
