using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.FeedInfos;

public class GetFeedInfoByProductEndpoint(LivestockDbContext db) : Endpoint<GetFeedInfoByProductRequest, FeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/FeedInfos/ByProduct");
        AllowAnonymous();
        Tags("FeedInfos");
    }

    public override async Task HandleAsync(GetFeedInfoByProductRequest req, CancellationToken ct)
    {
        var f = await db.FeedInfos.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (f is null) { AddError(LivestockErrors.FeedInfoErrors.FeedInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(FeedInfoMapper.Map(f), 200, ct);
    }
}

public class GetFeedInfoEndpoint(LivestockDbContext db) : Endpoint<GetFeedInfoRequest, FeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/FeedInfos/Detail");
        AllowAnonymous();
        Tags("FeedInfos");
    }

    public override async Task HandleAsync(GetFeedInfoRequest req, CancellationToken ct)
    {
        var f = await db.FeedInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (f is null) { AddError(LivestockErrors.FeedInfoErrors.FeedInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(FeedInfoMapper.Map(f), 200, ct);
    }
}

public class CreateFeedInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateFeedInfoRequest, FeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/FeedInfos/Create");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("FeedInfos");
    }

    public override async Task HandleAsync(CreateFeedInfoRequest req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && !p.IsDeleted, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        { AddError(LivestockErrors.FeedInfoErrors.FeedInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var exists = await db.FeedInfos.AnyAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (exists) { AddError(LivestockErrors.FeedInfoErrors.FeedInfoAlreadyExists); await SendErrorsAsync(409, ct); return; }

        var f = new FeedInfo
        {
            ProductId = req.ProductId, FeedType = req.FeedType, FeedForm = req.FeedForm,
            TargetSpecies = req.TargetSpecies, ProteinPercentage = req.ProteinPercentage,
            FatPercentage = req.FatPercentage, FiberPercentage = req.FiberPercentage,
            MoisturePercentage = req.MoisturePercentage, Ingredients = req.Ingredients,
            Additives = req.Additives, ManufactureDate = req.ManufactureDate, ExpiryDate = req.ExpiryDate,
            CertificationInfo = req.CertificationInfo, IsOrganic = req.IsOrganic, PackageSizeKg = req.PackageSizeKg
        };
        db.FeedInfos.Add(f);
        await db.SaveChangesAsync(ct);
        await SendAsync(FeedInfoMapper.Map(f), 201, ct);
    }
}

public class UpdateFeedInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateFeedInfoRequest, FeedInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/FeedInfos/Update");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("FeedInfos");
    }

    public override async Task HandleAsync(UpdateFeedInfoRequest req, CancellationToken ct)
    {
        var f = await db.FeedInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (f is null) { AddError(LivestockErrors.FeedInfoErrors.FeedInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || f.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.FeedInfoErrors.FeedInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        f.FeedType = req.FeedType; f.FeedForm = req.FeedForm; f.TargetSpecies = req.TargetSpecies;
        f.ProteinPercentage = req.ProteinPercentage; f.FatPercentage = req.FatPercentage;
        f.FiberPercentage = req.FiberPercentage; f.MoisturePercentage = req.MoisturePercentage;
        f.Ingredients = req.Ingredients; f.Additives = req.Additives;
        f.ManufactureDate = req.ManufactureDate; f.ExpiryDate = req.ExpiryDate;
        f.CertificationInfo = req.CertificationInfo; f.IsOrganic = req.IsOrganic; f.PackageSizeKg = req.PackageSizeKg;
        f.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(FeedInfoMapper.Map(f), 200, ct);
    }
}

public class DeleteFeedInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteFeedInfoRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/FeedInfos/Delete");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("FeedInfos");
    }

    public override async Task HandleAsync(DeleteFeedInfoRequest req, CancellationToken ct)
    {
        var f = await db.FeedInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (f is null) { AddError(LivestockErrors.FeedInfoErrors.FeedInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || f.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.FeedInfoErrors.FeedInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        f.IsDeleted = true; f.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class FeedInfoMapper
{
    internal static FeedInfoDetail Map(FeedInfo f) => new(
        f.Id, f.ProductId, f.FeedType, f.FeedForm, f.TargetSpecies,
        f.ProteinPercentage, f.FatPercentage, f.FiberPercentage, f.MoisturePercentage,
        f.Ingredients, f.Additives, f.ManufactureDate, f.ExpiryDate,
        f.CertificationInfo, f.IsOrganic, f.PackageSizeKg, f.CreatedAt);
}
