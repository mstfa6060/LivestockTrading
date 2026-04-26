using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.AnimalInfos;

public class GetAnimalInfoByProductEndpoint(LivestockDbContext db) : Endpoint<GetAnimalInfoByProductRequest, AnimalInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/AnimalInfos/ByProduct");
        AllowAnonymous();
        Tags("AnimalInfos");
    }

    public override async Task HandleAsync(GetAnimalInfoByProductRequest req, CancellationToken ct)
    {
        var a = await db.AnimalInfos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == req.ProductId && !x.IsDeleted, ct);
        if (a is null) { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(AnimalInfoMapper.Map(a), 200, ct);
    }
}

public class GetAnimalInfoEndpoint(LivestockDbContext db) : Endpoint<GetAnimalInfoRequest, AnimalInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/AnimalInfos/Detail");
        AllowAnonymous();
        Tags("AnimalInfos");
    }

    public override async Task HandleAsync(GetAnimalInfoRequest req, CancellationToken ct)
    {
        var a = await db.AnimalInfos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (a is null) { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(AnimalInfoMapper.Map(a), 200, ct);
    }
}

public class CreateAnimalInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateAnimalInfoRequest, AnimalInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/AnimalInfos/Create");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("AnimalInfos");
    }

    public override async Task HandleAsync(CreateAnimalInfoRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId && !p.IsDeleted, ct);
        if (product is null) { AddError(LivestockErrors.ProductErrors.ProductNotFound); await SendErrorsAsync(404, ct); return; }
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || product.SellerId != seller.Id))
        { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var exists = await db.AnimalInfos.AnyAsync(a => a.ProductId == req.ProductId && !a.IsDeleted, ct);
        if (exists) { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoAlreadyExists); await SendErrorsAsync(409, ct); return; }

        var a = new AnimalInfo
        {
            ProductId = req.ProductId, Species = req.Species, Breed = req.Breed,
            Gender = req.Gender, AgeMonths = req.AgeMonths, WeightKg = req.WeightKg,
            HealthStatus = req.HealthStatus, Purpose = req.Purpose,
            IsVaccinated = req.IsVaccinated, HasHealthCertificate = req.HasHealthCertificate,
            EarTagNumber = req.EarTagNumber, PassportNumber = req.PassportNumber, Color = req.Color,
            IsPregnant = req.IsPregnant, BreedingHistory = req.BreedingHistory
        };
        db.AnimalInfos.Add(a);
        await db.SaveChangesAsync(ct);
        await SendAsync(AnimalInfoMapper.Map(a),201, ct);
    }
}

public class UpdateAnimalInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateAnimalInfoRequest, AnimalInfoDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/AnimalInfos/Update");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("AnimalInfos");
    }

    public override async Task HandleAsync(UpdateAnimalInfoRequest req, CancellationToken ct)
    {
        var a = await db.AnimalInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (a is null) { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || a.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        a.Species = req.Species; a.Breed = req.Breed; a.Gender = req.Gender;
        a.AgeMonths = req.AgeMonths; a.WeightKg = req.WeightKg; a.HealthStatus = req.HealthStatus;
        a.Purpose = req.Purpose; a.IsVaccinated = req.IsVaccinated; a.HasHealthCertificate = req.HasHealthCertificate;
        a.EarTagNumber = req.EarTagNumber; a.PassportNumber = req.PassportNumber; a.Color = req.Color;
        a.IsPregnant = req.IsPregnant; a.BreedingHistory = req.BreedingHistory;
        a.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(AnimalInfoMapper.Map(a),200, ct);
    }
}

public class DeleteAnimalInfoEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteAnimalInfoRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/AnimalInfos/Delete");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("AnimalInfos");
    }

    public override async Task HandleAsync(DeleteAnimalInfoRequest req, CancellationToken ct)
    {
        var a = await db.AnimalInfos.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (a is null) { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || a.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        a.IsDeleted = true; a.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class AnimalInfoMapper
{
    internal static AnimalInfoDetail Map(AnimalInfo a) => new(
        a.Id, a.ProductId, a.Species, a.Breed, a.Gender, a.AgeMonths, a.WeightKg,
        a.HealthStatus, a.Purpose, a.IsVaccinated, a.HasHealthCertificate,
        a.EarTagNumber, a.PassportNumber, a.Color, a.IsPregnant, a.BreedingHistory, a.CreatedAt);
}
