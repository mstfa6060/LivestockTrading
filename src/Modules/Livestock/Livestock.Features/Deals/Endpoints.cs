using FastEndpoints;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Deals;

public class GetMyDealsEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<List<DealListItem>>
{
    public override void Configure()
    {
        Get("/Deals/My");
        Tags("Deals");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        var sellerId = seller?.Id ?? Guid.Empty;

        var deals = await db.Deals
            .AsNoTracking()
            .Include(d => d.Product)
            .Where(d => d.BuyerUserId == user.UserId || (seller != null && d.SellerId == sellerId))
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DealListItem(d.Id, d.OfferId, d.ProductId, d.Product.Title, d.BuyerUserId, d.SellerId, d.AgreePrice, d.CurrencyCode, d.Quantity, d.Status, d.DeliveryMethod, d.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(deals, 200, ct);
    }
}

public class GetDealEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<GetDealRequest, DealDetail>
{
    public override void Configure()
    {
        Get("/Deals/{Id}");
        Tags("Deals");
    }

    public override async Task HandleAsync(GetDealRequest req, CancellationToken ct)
    {
        var deal = await db.Deals.AsNoTracking().Include(d => d.Product).FirstOrDefaultAsync(d => d.Id == req.Id, ct);
        if (deal is null)
        {
            AddError(LivestockErrors.DealErrors.DealNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (deal.BuyerUserId != user.UserId && (seller is null || deal.SellerId != seller.Id))
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        await SendAsync(new DealDetail(deal.Id, deal.OfferId, deal.ProductId, deal.Product.Title, deal.BuyerUserId, deal.SellerId, deal.AgreePrice, deal.CurrencyCode, deal.Quantity, deal.Status, deal.DeliveryMethod, deal.Notes, deal.CompletedAt, deal.CreatedAt), 200, ct);
    }
}

public class GetAllDealsEndpoint(LivestockDbContext db) : Endpoint<GetAllDealsRequest, List<DealListItem>>
{
    public override void Configure()
    {
        Get("/Admin/Deals");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Deals");
    }

    public override async Task HandleAsync(GetAllDealsRequest req, CancellationToken ct)
    {
        var pageSize = Math.Min(req.PageSize, 100);
        var skip = (req.Page - 1) * pageSize;

        var deals = await db.Deals
            .AsNoTracking()
            .Include(d => d.Product)
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip).Take(pageSize)
            .Select(d => new DealListItem(d.Id, d.OfferId, d.ProductId, d.Product.Title, d.BuyerUserId, d.SellerId, d.AgreePrice, d.CurrencyCode, d.Quantity, d.Status, d.DeliveryMethod, d.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(deals, 200, ct);
    }
}

public class PickDealsEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DealPickRequest, List<DealPickItem>>
{
    public override void Configure()
    {
        Post("/Deals/Pick");
        Tags("Deals");
    }

    public override async Task HandleAsync(DealPickRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        var sellerId = seller?.Id ?? Guid.Empty;

        var query = db.Deals
            .AsNoTracking()
            .Include(d => d.Product)
            .Where(d => d.BuyerUserId == user.UserId || (seller != null && d.SellerId == sellerId));

        if (req.SelectedIds != null && req.SelectedIds.Count > 0)
        {
            query = query.Where(d => req.SelectedIds.Contains(d.Id));
        }

        var deals = await query
            .OrderByDescending(d => d.CreatedAt)
            .Take(req.Limit > 0 ? req.Limit : 10)
            .Select(d => new DealPickItem(d.Id, d.Product.Title, d.Status, d.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(deals, 200, ct);
    }
}

public class CancelDealEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CancelDealRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Deals/{Id}/Cancel");
        Tags("Deals");
    }

    public override async Task HandleAsync(CancelDealRequest req, CancellationToken ct)
    {
        var deal = await db.Deals.FirstOrDefaultAsync(d => d.Id == req.Id, ct);
        if (deal is null)
        {
            AddError(LivestockErrors.DealErrors.DealNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (deal.BuyerUserId != user.UserId && (seller is null || deal.SellerId != seller.Id))
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        if (deal.Status == DealStatus.Completed)
        {
            AddError(LivestockErrors.DealErrors.DealAlreadyCompleted);
            await SendErrorsAsync(409, ct);
            return;
        }

        if (deal.Status == DealStatus.Cancelled)
        {
            AddError(LivestockErrors.DealErrors.DealAlreadyCancelled);
            await SendErrorsAsync(409, ct);
            return;
        }

        deal.Status = DealStatus.Cancelled;
        deal.CancellationReason = req.Reason;
        deal.CancelledAt = DateTime.UtcNow;
        deal.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class UpdateDealStatusEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateDealStatusRequest, DealDetail>
{
    public override void Configure()
    {
        Patch("/Deals/{Id}/Status");
        Tags("Deals");
    }

    public override async Task HandleAsync(UpdateDealStatusRequest req, CancellationToken ct)
    {
        var deal = await db.Deals.Include(d => d.Product).FirstOrDefaultAsync(d => d.Id == req.Id, ct);
        if (deal is null)
        {
            AddError(LivestockErrors.DealErrors.DealNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        var seller = await db.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);
        if (deal.BuyerUserId != user.UserId && (seller is null || deal.SellerId != seller.Id))
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        if (deal.Status == DealStatus.Completed)
        {
            AddError(LivestockErrors.DealErrors.DealAlreadyCompleted);
            await SendErrorsAsync(409, ct);
            return;
        }

        if (deal.Status == DealStatus.Cancelled)
        {
            AddError(LivestockErrors.DealErrors.DealAlreadyCancelled);
            await SendErrorsAsync(409, ct);
            return;
        }

        deal.Status = req.Status;
        if (req.DeliveryMethod.HasValue)
        {
            deal.DeliveryMethod = req.DeliveryMethod.Value;
        }

        if (req.Notes is not null)
        {
            deal.Notes = req.Notes;
        }

        if (req.Status == DealStatus.Completed)
        {
            deal.CompletedAt = DateTime.UtcNow;
        }

        if (req.Status == DealStatus.Cancelled)
        {
            deal.CancelledAt = DateTime.UtcNow;
        }
        deal.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new DealDetail(deal.Id, deal.OfferId, deal.ProductId, deal.Product.Title, deal.BuyerUserId, deal.SellerId, deal.AgreePrice, deal.CurrencyCode, deal.Quantity, deal.Status, deal.DeliveryMethod, deal.Notes, deal.CompletedAt, deal.CreatedAt), 200, ct);
    }
}
