using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.HealthRecords;

public class GetHealthRecordsEndpoint(LivestockDbContext db) : Endpoint<GetHealthRecordsRequest, List<HealthRecordDetail>>
{
    public override void Configure()
    {
        Post("/livestocktrading/HealthRecords/ByAnimal");
        AllowAnonymous();
        Tags("HealthRecords");
    }

    public override async Task HandleAsync(GetHealthRecordsRequest req, CancellationToken ct)
    {
        var records = await db.HealthRecords.AsNoTracking()
            .Where(r => r.AnimalInfoId == req.AnimalInfoId && !r.IsDeleted)
            .OrderByDescending(r => r.TreatmentDate)
            .Select(r => HealthRecordMapper.Map(r))
            .ToListAsync(ct);
        await SendAsync(records, 200, ct);
    }
}

public class GetHealthRecordEndpoint(LivestockDbContext db) : Endpoint<GetHealthRecordRequest, HealthRecordDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/HealthRecords/Detail");
        AllowAnonymous();
        Tags("HealthRecords");
    }

    public override async Task HandleAsync(GetHealthRecordRequest req, CancellationToken ct)
    {
        var r = await db.HealthRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (r is null) { AddError(LivestockErrors.HealthRecordErrors.HealthRecordNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(HealthRecordMapper.Map(r), 200, ct);
    }
}

public class CreateHealthRecordEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateHealthRecordRequest, HealthRecordDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/HealthRecords/Create");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("HealthRecords");
    }

    public override async Task HandleAsync(CreateHealthRecordRequest req, CancellationToken ct)
    {
        var animalInfo = await db.AnimalInfos.Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.Id == req.AnimalInfoId && !a.IsDeleted, ct);
        if (animalInfo is null) { AddError(LivestockErrors.AnimalInfoErrors.AnimalInfoNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || animalInfo.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.HealthRecordErrors.HealthRecordNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        var r = new HealthRecord
        {
            AnimalInfoId = req.AnimalInfoId, Diagnosis = req.Diagnosis, Treatment = req.Treatment,
            VetName = req.VetName, VetClinic = req.VetClinic, TreatmentDate = req.TreatmentDate, Notes = req.Notes
        };
        db.HealthRecords.Add(r);
        await db.SaveChangesAsync(ct);
        await SendAsync(HealthRecordMapper.Map(r), 201, ct);
    }
}

public class UpdateHealthRecordEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateHealthRecordRequest, HealthRecordDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/HealthRecords/Update");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("HealthRecords");
    }

    public override async Task HandleAsync(UpdateHealthRecordRequest req, CancellationToken ct)
    {
        var r = await db.HealthRecords.Include(x => x.AnimalInfo).ThenInclude(a => a.Product)
            .FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (r is null) { AddError(LivestockErrors.HealthRecordErrors.HealthRecordNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || r.AnimalInfo.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.HealthRecordErrors.HealthRecordNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        r.Diagnosis = req.Diagnosis; r.Treatment = req.Treatment;
        r.VetName = req.VetName; r.VetClinic = req.VetClinic;
        r.TreatmentDate = req.TreatmentDate; r.Notes = req.Notes;
        r.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(HealthRecordMapper.Map(r), 200, ct);
    }
}

public class DeleteHealthRecordEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteHealthRecordRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/HealthRecords/Delete");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("HealthRecords");
    }

    public override async Task HandleAsync(DeleteHealthRecordRequest req, CancellationToken ct)
    {
        var r = await db.HealthRecords.Include(x => x.AnimalInfo).ThenInclude(a => a.Product)
            .FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (r is null) { AddError(LivestockErrors.HealthRecordErrors.HealthRecordNotFound); await SendErrorsAsync(404, ct); return; }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || r.AnimalInfo.Product.SellerId != seller.Id))
        { AddError(LivestockErrors.HealthRecordErrors.HealthRecordNotOwnedBySeller); await SendErrorsAsync(403, ct); return; }

        r.IsDeleted = true; r.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class HealthRecordMapper
{
    internal static HealthRecordDetail Map(HealthRecord r) => new(
        r.Id, r.AnimalInfoId, r.Diagnosis, r.Treatment,
        r.VetName, r.VetClinic, r.TreatmentDate, r.Notes, r.CreatedAt);
}
