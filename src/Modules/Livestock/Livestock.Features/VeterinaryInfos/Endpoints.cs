using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.VeterinaryInfos;

public class GetVeterinaryInfoByProductEndpoint(LivestockDbContext db) : Endpoint<GetVeterinaryInfoByProductRequest, VeterinaryInfoDetail>
{
    public override void Configure()
    {
        Get("/VeterinaryInfos/ByProduct/{ProductId}");
        AllowAnonymous();
        Tags("VeterinaryInfos");
    }

    public override async Task HandleAsync(GetVeterinaryInfoByProductRequest req, CancellationToken ct)
    {
        var v = await db.VeterinaryInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(VeterinaryInfoMapper.Map(v), 200, ct);
    }
}

public class GetVeterinaryInfoEndpoint(LivestockDbContext db) : Endpoint<GetVeterinaryInfoRequest, VeterinaryInfoDetail>
{
    public override void Configure()
    {
        Get("/VeterinaryInfos/{Id}");
        AllowAnonymous();
        Tags("VeterinaryInfos");
    }

    public override async Task HandleAsync(GetVeterinaryInfoRequest req, CancellationToken ct)
    {
        var v = await db.VeterinaryInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(VeterinaryInfoMapper.Map(v), 200, ct);
    }
}

public class CreateVeterinaryInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateVeterinaryInfoRequest, VeterinaryInfoDetail>
{
    public override void Configure()
    {
        Post("/VeterinaryInfos");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("VeterinaryInfos");
    }

    public override async Task HandleAsync(CreateVeterinaryInfoRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && !p.IsDeleted, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var exists = await db.VeterinaryInfos.AnyAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (exists) { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoAlreadyExists); await SendErrorsAsync(409, ct); return; }

        var v = new VeterinaryInfo
        {
            ProductId = req.ProductId, ProductType = req.ProductType, AdministrationRoute = req.AdministrationRoute,
            ActiveSubstance = req.ActiveSubstance, Concentration = req.Concentration,
            TargetSpecies = req.TargetSpecies, Indications = req.Indications, Contraindications = req.Contraindications,
            Dosage = req.Dosage, WithholdingPeriodDays = req.WithholdingPeriodDays,
            RequiresPrescription = req.RequiresPrescription, StorageConditions = req.StorageConditions,
            RegistrationNumber = req.RegistrationNumber, ExpiryDate = req.ExpiryDate, Manufacturer = req.Manufacturer
        };
        db.VeterinaryInfos.Add(v);
        await db.SaveChangesAsync(ct);
        await SendAsync(VeterinaryInfoMapper.Map(v), 201, ct);
    }
}

public class UpdateVeterinaryInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateVeterinaryInfoRequest, VeterinaryInfoDetail>
{
    public override void Configure()
    {
        Put("/VeterinaryInfos/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("VeterinaryInfos");
    }

    public override async Task HandleAsync(UpdateVeterinaryInfoRequest req, CancellationToken ct)
    {
        var v = await db.VeterinaryInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || v.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        v.ProductType = req.ProductType; v.AdministrationRoute = req.AdministrationRoute;
        v.ActiveSubstance = req.ActiveSubstance; v.Concentration = req.Concentration;
        v.TargetSpecies = req.TargetSpecies; v.Indications = req.Indications; v.Contraindications = req.Contraindications;
        v.Dosage = req.Dosage; v.WithholdingPeriodDays = req.WithholdingPeriodDays;
        v.RequiresPrescription = req.RequiresPrescription; v.StorageConditions = req.StorageConditions;
        v.RegistrationNumber = req.RegistrationNumber; v.ExpiryDate = req.ExpiryDate; v.Manufacturer = req.Manufacturer;
        v.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(VeterinaryInfoMapper.Map(v), 200, ct);
    }
}

public class DeleteVeterinaryInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteVeterinaryInfoRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/VeterinaryInfos/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("VeterinaryInfos");
    }

    public override async Task HandleAsync(DeleteVeterinaryInfoRequest req, CancellationToken ct)
    {
        var v = await db.VeterinaryInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || v.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.VeterinaryInfoErrors.VeterinaryInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        v.IsDeleted = true; v.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class VeterinaryInfoMapper
{
    internal static VeterinaryInfoDetail Map(VeterinaryInfo v) => new(
        v.Id, v.ProductId, v.ProductType, v.AdministrationRoute,
        v.ActiveSubstance, v.Concentration, v.TargetSpecies, v.Indications, v.Contraindications,
        v.Dosage, v.WithholdingPeriodDays, v.RequiresPrescription, v.StorageConditions,
        v.RegistrationNumber, v.ExpiryDate, v.Manufacturer, v.CreatedAt);
}
