using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.SeedInfos;

public class GetSeedInfoByProductEndpoint(LivestockDbContext db) : Endpoint<GetSeedInfoByProductRequest, SeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/SeedInfos/ByProduct");
        AllowAnonymous();
        Tags("SeedInfos");
    }

    public override async Task HandleAsync(GetSeedInfoByProductRequest req, CancellationToken ct)
    {
        var s = await db.SeedInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (s is null) { AddError(LivestockErrors.SeedInfoErrors.SeedInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(SeedInfoMapper.Map(s), 200, ct);
    }
}

public class GetSeedInfoEndpoint(LivestockDbContext db) : Endpoint<GetSeedInfoRequest, SeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/SeedInfos/Detail");
        AllowAnonymous();
        Tags("SeedInfos");
    }

    public override async Task HandleAsync(GetSeedInfoRequest req, CancellationToken ct)
    {
        var s = await db.SeedInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (s is null) { AddError(LivestockErrors.SeedInfoErrors.SeedInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(SeedInfoMapper.Map(s), 200, ct);
    }
}

public class CreateSeedInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateSeedInfoRequest, SeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/SeedInfos/Create");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("SeedInfos");
    }

    public override async Task HandleAsync(CreateSeedInfoRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && !p.IsDeleted, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        { AddError(LivestockErrors.SeedInfoErrors.SeedInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var exists = await db.SeedInfos.AnyAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (exists) { AddError(LivestockErrors.SeedInfoErrors.SeedInfoAlreadyExists); await SendErrorsAsync(409, ct); return; }

        var s = new SeedInfo
        {
            ProductId = req.ProductId, SeedType = req.SeedType, Variety = req.Variety,
            ScientificName = req.ScientificName, GerminationRatePercent = req.GerminationRatePercent,
            DaysToMaturity = req.DaysToMaturity, PackageSizeGrams = req.PackageSizeGrams,
            HarvestDate = req.HarvestDate, ExpiryDate = req.ExpiryDate,
            IsHybrid = req.IsHybrid, IsOrganic = req.IsOrganic, IsGmoFree = req.IsGmoFree,
            SuitableClimate = req.SuitableClimate, PlantingInstructions = req.PlantingInstructions,
            CertificationInfo = req.CertificationInfo
        };
        db.SeedInfos.Add(s);
        await db.SaveChangesAsync(ct);
        await SendAsync(SeedInfoMapper.Map(s), 201, ct);
    }
}

public class UpdateSeedInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateSeedInfoRequest, SeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/SeedInfos/Update");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("SeedInfos");
    }

    public override async Task HandleAsync(UpdateSeedInfoRequest req, CancellationToken ct)
    {
        var s = await db.SeedInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (s is null) { AddError(LivestockErrors.SeedInfoErrors.SeedInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(sl => sl.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || s.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.SeedInfoErrors.SeedInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        s.SeedType = req.SeedType; s.Variety = req.Variety; s.ScientificName = req.ScientificName;
        s.GerminationRatePercent = req.GerminationRatePercent; s.DaysToMaturity = req.DaysToMaturity;
        s.PackageSizeGrams = req.PackageSizeGrams; s.HarvestDate = req.HarvestDate; s.ExpiryDate = req.ExpiryDate;
        s.IsHybrid = req.IsHybrid; s.IsOrganic = req.IsOrganic; s.IsGmoFree = req.IsGmoFree;
        s.SuitableClimate = req.SuitableClimate; s.PlantingInstructions = req.PlantingInstructions;
        s.CertificationInfo = req.CertificationInfo;
        s.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(SeedInfoMapper.Map(s), 200, ct);
    }
}

public class DeleteSeedInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteSeedInfoRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/SeedInfos/Delete");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("SeedInfos");
    }

    public override async Task HandleAsync(DeleteSeedInfoRequest req, CancellationToken ct)
    {
        var s = await db.SeedInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (s is null) { AddError(LivestockErrors.SeedInfoErrors.SeedInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(sl => sl.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || s.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.SeedInfoErrors.SeedInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        s.IsDeleted = true; s.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class SeedInfoMapper
{
    internal static SeedInfoDetail Map(SeedInfo s) => new(
        s.Id, s.ProductId, s.SeedType, s.Variety, s.ScientificName,
        s.GerminationRatePercent, s.DaysToMaturity, s.PackageSizeGrams,
        s.HarvestDate, s.ExpiryDate, s.IsHybrid, s.IsOrganic, s.IsGmoFree,
        s.SuitableClimate, s.PlantingInstructions, s.CertificationInfo, s.CreatedAt);
}
