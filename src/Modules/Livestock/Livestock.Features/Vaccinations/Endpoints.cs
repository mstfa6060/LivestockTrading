using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Vaccinations;

public class GetVaccinationsEndpoint(LivestockDbContext db) : Endpoint<GetVaccinationsRequest, List<VaccinationDetail>>
{
    public override void Configure()
    {
        Get("/Vaccinations/ByAnimal/{AnimalInfoId}");
        AllowAnonymous();
        Tags("Vaccinations");
    }

    public override async Task HandleAsync(GetVaccinationsRequest req, CancellationToken ct)
    {
        var records = await db.Vaccinations.AsNoTracking()
            .Where(v => v.AnimalInfoId == req.AnimalInfoId && !v.IsDeleted)
            .OrderByDescending(v => v.AdministeredAt)
            .Select(v => VaccinationMapper.Map(v))
            .ToListAsync(ct);
        await SendAsync(records, 200, ct);
    }
}

public class GetVaccinationEndpoint(LivestockDbContext db) : Endpoint<GetVaccinationRequest, VaccinationDetail>
{
    public override void Configure()
    {
        Get("/Vaccinations/{Id}");
        AllowAnonymous();
        Tags("Vaccinations");
    }

    public override async Task HandleAsync(GetVaccinationRequest req, CancellationToken ct)
    {
        var v = await db.Vaccinations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.VaccinationErrors.VaccinationNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(VaccinationMapper.Map(v), 200, ct);
    }
}

public class CreateVaccinationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateVaccinationRequest, VaccinationDetail>
{
    public override void Configure()
    {
        Post("/Vaccinations");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Vaccinations");
    }

    public override async Task HandleAsync(CreateVaccinationRequest req, CancellationToken ct)
    {
        var animalInfo = await db.AnimalInfos.Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.Id == req.AnimalInfoId && !a.IsDeleted, ct);
        if (animalInfo is null) { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || animalInfo.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.VaccinationErrors.VaccinationNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var v = new Vaccination
        {
            AnimalInfoId = req.AnimalInfoId, VaccineName = req.VaccineName, Manufacturer = req.Manufacturer,
            BatchNumber = req.BatchNumber, AdministeredAt = req.AdministeredAt, NextDueAt = req.NextDueAt,
            VetName = req.VetName, Notes = req.Notes
        };
        db.Vaccinations.Add(v);
        await db.SaveChangesAsync(ct);
        await SendAsync(VaccinationMapper.Map(v), 201, ct);
    }
}

public class UpdateVaccinationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateVaccinationRequest, VaccinationDetail>
{
    public override void Configure()
    {
        Put("/Vaccinations/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Vaccinations");
    }

    public override async Task HandleAsync(UpdateVaccinationRequest req, CancellationToken ct)
    {
        var v = await db.Vaccinations.Include(x => x.AnimalInfo).ThenInclude(a => a.Product)
            .FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.VaccinationErrors.VaccinationNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || v.AnimalInfo.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.VaccinationErrors.VaccinationNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        v.VaccineName = req.VaccineName; v.Manufacturer = req.Manufacturer; v.BatchNumber = req.BatchNumber;
        v.AdministeredAt = req.AdministeredAt; v.NextDueAt = req.NextDueAt;
        v.VetName = req.VetName; v.Notes = req.Notes;
        v.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(VaccinationMapper.Map(v), 200, ct);
    }
}

public class DeleteVaccinationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteVaccinationRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Vaccinations/{Id}");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Vaccinations");
    }

    public override async Task HandleAsync(DeleteVaccinationRequest req, CancellationToken ct)
    {
        var v = await db.Vaccinations.Include(x => x.AnimalInfo).ThenInclude(a => a.Product)
            .FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (v is null) { AddError(LivestockErrors.VaccinationErrors.VaccinationNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || v.AnimalInfo.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.VaccinationErrors.VaccinationNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        v.IsDeleted = true; v.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class VaccinationMapper
{
    internal static VaccinationDetail Map(Vaccination v) => new(
        v.Id, v.AnimalInfoId, v.VaccineName, v.Manufacturer, v.BatchNumber,
        v.AdministeredAt, v.NextDueAt, v.VetName, v.Notes, v.CreatedAt);
}
