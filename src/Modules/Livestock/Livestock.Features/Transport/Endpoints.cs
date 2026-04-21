using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Transport;

public class GetMyTransportRequestsEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<List<TransportRequestListItem>>
{
    public override void Configure()
    {
        Get("/TransportRequests/My");
        Tags("Transport");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var requests = await db.TransportRequests
            .AsNoTracking()
            .Where(r => r.RequesterUserId == user.UserId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new TransportRequestListItem(r.Id, r.RequesterUserId, r.PickupCountryCode, r.PickupCity, r.DeliveryCountryCode, r.DeliveryCity, r.TransportType, r.Status, r.Budget, r.CurrencyCode, r.PickupDate, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(requests, 200, ct);
    }
}

public class GetAllOpenTransportRequestsEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<TransportRequestListItem>>
{
    public override void Configure()
    {
        Get("/TransportRequests/Open");
        Tags("Transport");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var requests = await db.TransportRequests
            .AsNoTracking()
            .Where(r => r.Status == TransportRequestStatus.InPool || r.Status == TransportRequestStatus.ReceivingOffers)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new TransportRequestListItem(r.Id, r.RequesterUserId, r.PickupCountryCode, r.PickupCity, r.DeliveryCountryCode, r.DeliveryCity, r.TransportType, r.Status, r.Budget, r.CurrencyCode, r.PickupDate, r.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(requests, 200, ct);
    }
}

public class GetTransportRequestEndpoint(LivestockDbContext db) : Endpoint<GetTransportRequestRequest, TransportRequestDetail>
{
    public override void Configure()
    {
        Get("/TransportRequests/{Id}");
        Tags("Transport");
    }

    public override async Task HandleAsync(GetTransportRequestRequest req, CancellationToken ct)
    {
        var r = await db.TransportRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (r is null)
        {
            AddError(LivestockErrors.TransportErrors.TransportRequestNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new TransportRequestDetail(r.Id, r.RequesterUserId, r.SellerId, r.ProductId, r.AssignedTransporterId, r.PickupCountryCode, r.PickupCity, r.PickupAddress, r.DeliveryCountryCode, r.DeliveryCity, r.DeliveryAddress, r.TransportType, r.Status, r.CargoDescription, r.AnimalCount, r.EstimatedWeightKg, r.PickupDate, r.SpecialRequirements, r.Budget, r.CurrencyCode, r.CreatedAt), 200, ct);
    }
}

public class CreateTransportRequestEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<CreateTransportRequestRequest, TransportRequestDetail>
{
    public override void Configure()
    {
        Post("/TransportRequests");
        Tags("Transport");
    }

    public override async Task HandleAsync(CreateTransportRequestRequest req, CancellationToken ct)
    {
        var seller = await db.Sellers.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == user.UserId, ct);

        var request = new TransportRequest
        {
            RequesterUserId = user.UserId,
            SellerId = seller?.Id,
            ProductId = req.ProductId,
            PickupCountryCode = req.PickupCountryCode,
            PickupCity = req.PickupCity,
            PickupAddress = req.PickupAddress,
            PickupLatitude = req.PickupLatitude,
            PickupLongitude = req.PickupLongitude,
            DeliveryCountryCode = req.DeliveryCountryCode,
            DeliveryCity = req.DeliveryCity,
            DeliveryAddress = req.DeliveryAddress,
            DeliveryLatitude = req.DeliveryLatitude,
            DeliveryLongitude = req.DeliveryLongitude,
            TransportType = req.TransportType,
            CargoDescription = req.CargoDescription,
            AnimalCount = req.AnimalCount,
            EstimatedWeightKg = req.EstimatedWeightKg,
            PickupDate = req.PickupDate,
            SpecialRequirements = req.SpecialRequirements,
            Budget = req.Budget,
            CurrencyCode = req.CurrencyCode,
            Status = TransportRequestStatus.InPool,
            PoolExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.TransportRequests.Add(request);
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(TransportRequestCreatedEvent.Subject, new TransportRequestCreatedEvent
        {
            TransportRequestId = request.Id,
            RequesterUserId = request.RequesterUserId,
            PickupCountryCode = request.PickupCountryCode,
            PickupCity = request.PickupCity,
            DeliveryCountryCode = request.DeliveryCountryCode,
            DeliveryCity = request.DeliveryCity
        }, ct);

        await SendAsync(new TransportRequestDetail(request.Id, request.RequesterUserId, request.SellerId, request.ProductId, null, request.PickupCountryCode, request.PickupCity, request.PickupAddress, request.DeliveryCountryCode, request.DeliveryCity, request.DeliveryAddress, request.TransportType, request.Status, request.CargoDescription, request.AnimalCount, request.EstimatedWeightKg, request.PickupDate, request.SpecialRequirements, request.Budget, request.CurrencyCode, request.CreatedAt), 201, ct);
    }
}

public class GetTransportOffersEndpoint(LivestockDbContext db) : Endpoint<GetTransportOffersByRequestRequest, List<TransportOfferListItem>>
{
    public override void Configure()
    {
        Get("/TransportRequests/{TransportRequestId}/Offers");
        Tags("Transport");
    }

    public override async Task HandleAsync(GetTransportOffersByRequestRequest req, CancellationToken ct)
    {
        var offers = await db.TransportOffers
            .AsNoTracking()
            .Include(o => o.Transporter)
            .Where(o => o.TransportRequestId == req.TransportRequestId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new TransportOfferListItem(o.Id, o.TransportRequestId, o.TransporterId, o.Transporter.CompanyName, o.Price, o.CurrencyCode, o.EstimatedDaysMin, o.EstimatedDaysMax, o.Status, o.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(offers, 200, ct);
    }
}

public class CreateTransportOfferEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<CreateTransportOfferRequest, TransportOfferListItem>
{
    public override void Configure()
    {
        Post("/TransportRequests/{TransportRequestId}/Offers");
        Roles("LivestockTrading.Transporter", "LivestockTrading.Admin");
        Tags("Transport");
    }

    public override async Task HandleAsync(CreateTransportOfferRequest req, CancellationToken ct)
    {
        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.UserId == user.UserId, ct);
        if (transporter is null || transporter.Status != TransporterStatus.Active)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotVerified);
            await SendErrorsAsync(403, ct);
            return;
        }

        var request = await db.TransportRequests.FirstOrDefaultAsync(r => r.Id == req.TransportRequestId, ct);
        if (request is null || (request.Status != TransportRequestStatus.InPool && request.Status != TransportRequestStatus.ReceivingOffers))
        {
            AddError(LivestockErrors.TransportErrors.TransportRequestNotOpen);
            await SendErrorsAsync(409, ct);
            return;
        }

        var exists = await db.TransportOffers.AnyAsync(o => o.TransportRequestId == req.TransportRequestId && o.TransporterId == transporter.Id, ct);
        if (exists)
        {
            AddError(LivestockErrors.TransportErrors.TransportOfferAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var offer = new TransportOffer
        {
            TransportRequestId = req.TransportRequestId,
            TransporterId = transporter.Id,
            Price = req.Price,
            CurrencyCode = req.CurrencyCode,
            Note = req.Note,
            EstimatedDaysMin = req.EstimatedDaysMin,
            EstimatedDaysMax = req.EstimatedDaysMax,
            Status = TransportOfferStatus.Pending,
            ExpiresAt = DateTime.UtcNow.AddDays(2)
        };

        if (request.Status == TransportRequestStatus.InPool)
        {
            request.Status = TransportRequestStatus.ReceivingOffers;
        }

        db.TransportOffers.Add(offer);
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(TransportOfferCreatedEvent.Subject, new TransportOfferCreatedEvent
        {
            TransportOfferId = offer.Id,
            TransportRequestId = offer.TransportRequestId,
            TransporterId = offer.TransporterId,
            RequesterUserId = request.RequesterUserId,
            Price = offer.Price,
            CurrencyCode = offer.CurrencyCode
        }, ct);

        await SendAsync(new TransportOfferListItem(offer.Id, offer.TransportRequestId, offer.TransporterId, transporter.CompanyName, offer.Price, offer.CurrencyCode, offer.EstimatedDaysMin, offer.EstimatedDaysMax, offer.Status, offer.CreatedAt), 201, ct);
    }
}

public class AcceptTransportOfferEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<AcceptTransportOfferRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Transport/Offers/{Id}/Accept");
        Tags("Transport");
    }

    public override async Task HandleAsync(AcceptTransportOfferRequest req, CancellationToken ct)
    {
        var offer = await db.TransportOffers.Include(o => o.TransportRequest).FirstOrDefaultAsync(o => o.Id == req.Id, ct);
        if (offer is null)
        {
            AddError(LivestockErrors.TransportErrors.TransportOfferNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (offer.TransportRequest.RequesterUserId != user.UserId)
        {
            AddError(LivestockErrors.Common.Unauthorized);
            await SendErrorsAsync(403, ct);
            return;
        }

        offer.Status = TransportOfferStatus.Accepted;
        offer.TransportRequest.Status = TransportRequestStatus.Assigned;
        offer.TransportRequest.AssignedTransporterId = offer.TransporterId;
        offer.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(TransportOfferAcceptedEvent.Subject, new TransportOfferAcceptedEvent
        {
            TransportOfferId = offer.Id,
            TransportRequestId = offer.TransportRequestId,
            TransporterId = offer.TransporterId,
            RequesterUserId = offer.TransportRequest.RequesterUserId
        }, ct);

        await SendNoContentAsync(ct);
    }
}

public class AddTrackingUpdateEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<AddTrackingUpdateRequest, TrackingUpdateItem>
{
    public override void Configure()
    {
        Post("/TransportRequests/{TransportRequestId}/Tracking");
        Roles("LivestockTrading.Transporter", "LivestockTrading.Admin");
        Tags("Transport");
    }

    public override async Task HandleAsync(AddTrackingUpdateRequest req, CancellationToken ct)
    {
        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.UserId == user.UserId, ct);
        if (transporter is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(403, ct);
            return;
        }

        var tracking = new TransportTracking
        {
            TransportRequestId = req.TransportRequestId,
            TransporterId = transporter.Id,
            Status = req.Status,
            Note = req.Note,
            Location = req.Location,
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            OccurredAt = DateTime.UtcNow
        };

        db.TransportTrackings.Add(tracking);
        await db.SaveChangesAsync(ct);

        await SendAsync(new TrackingUpdateItem(tracking.Id, tracking.Status, tracking.Note, tracking.Location, tracking.OccurredAt), 201, ct);
    }
}
