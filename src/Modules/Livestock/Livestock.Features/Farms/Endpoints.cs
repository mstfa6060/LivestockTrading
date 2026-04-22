using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Farms;

public class GetAllFarmsEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<List<FarmListItem>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Farms/All");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Farms");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(403, ct);
            return;
        }

        var farms = await db.Farms
            .AsNoTracking()
            .Where(f => f.SellerId == seller.Id)
            .Select(f => new FarmListItem(f.Id, f.SellerId, f.Name, f.FarmType, f.AreaHectares, f.CapacityHead, f.IsOrganic, f.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(farms, 200, ct);
    }
}

public class GetFarmEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetFarmRequest, FarmDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Farms/Detail");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Farms");
    }

    public override async Task HandleAsync(GetFarmRequest req, CancellationToken ct)
    {
        var f = await db.Farms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (f is null)
        {
            AddError(LivestockErrors.FarmErrors.FarmNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || f.SellerId != seller.Id))
        {
            AddError(LivestockErrors.FarmErrors.FarmNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        await SendAsync(new FarmDetail(f.Id, f.SellerId, f.Name, f.Description, f.FarmType, f.AreaHectares, f.CapacityHead, f.CertificationInfo, f.IsOrganic, f.WebsiteUrl, f.PhoneNumber, f.BucketId, f.CreatedAt), 200, ct);
    }
}

public class CreateFarmEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateFarmRequest, FarmDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Farms/Create");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Farms");
    }

    public override async Task HandleAsync(CreateFarmRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (seller is null)
        {
            AddError(LivestockErrors.SellerErrors.SellerNotFound);
            await SendErrorsAsync(403, ct);
            return;
        }

        var farm = new Farm
        {
            SellerId = seller.Id, Name = req.Name, Description = req.Description,
            FarmType = req.FarmType, AreaHectares = req.AreaHectares, CapacityHead = req.CapacityHead,
            CertificationInfo = req.CertificationInfo, IsOrganic = req.IsOrganic,
            WebsiteUrl = req.WebsiteUrl, PhoneNumber = req.PhoneNumber
        };

        db.Farms.Add(farm);
        await db.SaveChangesAsync(ct);

        await SendAsync(new FarmDetail(farm.Id, farm.SellerId, farm.Name, farm.Description, farm.FarmType, farm.AreaHectares, farm.CapacityHead, farm.CertificationInfo, farm.IsOrganic, farm.WebsiteUrl, farm.PhoneNumber, null, farm.CreatedAt), 201, ct);
    }
}

public class UpdateFarmEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateFarmRequest, FarmDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Farms/Update");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Farms");
    }

    public override async Task HandleAsync(UpdateFarmRequest req, CancellationToken ct)
    {
        var farm = await db.Farms.FirstOrDefaultAsync(f => f.Id == req.Id, ct);
        if (farm is null)
        {
            AddError(LivestockErrors.FarmErrors.FarmNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || farm.SellerId != seller.Id))
        {
            AddError(LivestockErrors.FarmErrors.FarmNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        farm.Name = req.Name; farm.Description = req.Description; farm.FarmType = req.FarmType;
        farm.AreaHectares = req.AreaHectares; farm.CapacityHead = req.CapacityHead;
        farm.CertificationInfo = req.CertificationInfo; farm.IsOrganic = req.IsOrganic;
        farm.WebsiteUrl = req.WebsiteUrl; farm.PhoneNumber = req.PhoneNumber;
        farm.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new FarmDetail(farm.Id, farm.SellerId, farm.Name, farm.Description, farm.FarmType, farm.AreaHectares, farm.CapacityHead, farm.CertificationInfo, farm.IsOrganic, farm.WebsiteUrl, farm.PhoneNumber, farm.BucketId, farm.CreatedAt), 200, ct);
    }
}

public class DeleteFarmEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteFarmRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Farms/Delete");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin");
        Tags("Farms");
    }

    public override async Task HandleAsync(DeleteFarmRequest req, CancellationToken ct)
    {
        var farm = await db.Farms.FirstOrDefaultAsync(f => f.Id == req.Id, ct);
        if (farm is null)
        {
            AddError(LivestockErrors.FarmErrors.FarmNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (!user.IsInRole("LivestockTrading.Admin") && (seller is null || farm.SellerId != seller.Id))
        {
            AddError(LivestockErrors.FarmErrors.FarmNotOwnedBySeller);
            await SendErrorsAsync(403, ct);
            return;
        }

        farm.IsDeleted = true; farm.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
