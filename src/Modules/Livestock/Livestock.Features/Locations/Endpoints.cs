using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;

namespace Livestock.Features.Locations;

public class GetLocationsByOwnerEndpoint(LivestockDbContext db) : Endpoint<GetLocationsByOwnerRequest, List<LocationDetail>>
{
    public override void Configure()
    {
        Post("/livestocktrading/Locations/ByOwner");
        AllowAnonymous();
        Tags("Locations");
    }

    public override async Task HandleAsync(GetLocationsByOwnerRequest req, CancellationToken ct)
    {
        var locations = await db.Locations.AsNoTracking()
            .Where(l => l.OwnerId == req.OwnerId && l.OwnerType == req.OwnerType && !l.IsDeleted)
            .Select(l => LocationMapper.Map(l))
            .ToListAsync(ct);
        await SendAsync(locations, 200, ct);
    }
}

public class GetLocationEndpoint(LivestockDbContext db) : Endpoint<GetLocationRequest, LocationDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Locations/Detail");
        AllowAnonymous();
        Tags("Locations");
    }

    public override async Task HandleAsync(GetLocationRequest req, CancellationToken ct)
    {
        var l = await db.Locations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (l is null) { AddError(LivestockErrors.LocationErrors.LocationNotFound); await SendErrorsAsync(404, ct); return; }
        await SendAsync(LocationMapper.Map(l), 200, ct);
    }
}

public class CreateLocationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<CreateLocationRequest, LocationDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Locations/Create");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin", "LivestockTrading.Buyer", "LivestockTrading.Transporter");
        Tags("Locations");
    }

    public override async Task HandleAsync(CreateLocationRequest req, CancellationToken ct)
    {
        // Normalize legacy field names the old frontend client still ships.
        var addressLine = !string.IsNullOrWhiteSpace(req.AddressLine)
            ? req.AddressLine
            : string.Join(", ",
                new[] { req.AddressLine1, req.AddressLine2 }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

        var district = !string.IsNullOrWhiteSpace(req.District)
            ? req.District
            : req.DistrictId?.ToString();

        var locationType = req.Type ?? req.LocationType;

        // OwnerId is NOT trusted from the body — the caller always owns the
        // row they create. OwnerType defaults to "Seller" for ProductLocation
        // (the listing-create flow); for other LocationTypes require the
        // caller to supply it explicitly.
        var ownerId = user.UserId;
        var ownerType = !string.IsNullOrWhiteSpace(req.OwnerType)
            ? req.OwnerType
            : locationType switch
            {
                LocationType.ProductLocation => "Seller",
                LocationType.FarmLocation => "Seller",
                LocationType.WarehouseLocation => "Seller",
                LocationType.ShippingAddress => "User",
                LocationType.BillingAddress => "User",
                LocationType.UserAddress => "User",
                _ => "User",
            };

        var l = new Location
        {
            CountryCode = req.CountryCode,
            CountryName = req.CountryName,
            State = req.State,
            City = req.City,
            District = district,
            PostalCode = req.PostalCode,
            AddressLine = addressLine,
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            LocationType = locationType,
            OwnerId = ownerId,
            OwnerType = ownerType,
        };
        db.Locations.Add(l);
        await db.SaveChangesAsync(ct);
        await SendAsync(LocationMapper.Map(l), 201, ct);
    }
}

public class UpdateLocationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateLocationRequest, LocationDetail>
{
    public override void Configure()
    {
        Post("/livestocktrading/Locations/Update");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin", "LivestockTrading.Buyer", "LivestockTrading.Transporter");
        Tags("Locations");
    }

    public override async Task HandleAsync(UpdateLocationRequest req, CancellationToken ct)
    {
        var l = await db.Locations.FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (l is null) { AddError(LivestockErrors.LocationErrors.LocationNotFound); await SendErrorsAsync(404, ct); return; }

        if (!user.IsInRole("LivestockTrading.Admin") && l.OwnerId != user.UserId)
        { AddError(LivestockErrors.Common.Unauthorized); await SendErrorsAsync(403, ct); return; }

        l.CountryCode = req.CountryCode; l.CountryName = req.CountryName;
        l.State = req.State; l.City = req.City; l.District = req.District;
        l.PostalCode = req.PostalCode; l.AddressLine = req.AddressLine;
        l.Latitude = req.Latitude; l.Longitude = req.Longitude;
        l.LocationType = req.LocationType;
        l.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendAsync(LocationMapper.Map(l), 200, ct);
    }
}

public class DeleteLocationEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<DeleteLocationRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/livestocktrading/Locations/Delete");
        Roles("LivestockTrading.Seller", "LivestockTrading.Admin", "LivestockTrading.Buyer", "LivestockTrading.Transporter");
        Tags("Locations");
    }

    public override async Task HandleAsync(DeleteLocationRequest req, CancellationToken ct)
    {
        var l = await db.Locations.FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (l is null) { AddError(LivestockErrors.LocationErrors.LocationNotFound); await SendErrorsAsync(404, ct); return; }

        if (!user.IsInRole("LivestockTrading.Admin") && l.OwnerId != user.UserId)
        { AddError(LivestockErrors.Common.Unauthorized); await SendErrorsAsync(403, ct); return; }

        l.IsDeleted = true; l.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

file static class LocationMapper
{
    internal static LocationDetail Map(Location l) => new(
        l.Id, l.CountryCode, l.CountryName, l.State, l.City, l.District, l.PostalCode,
        l.AddressLine, l.Latitude, l.Longitude, l.LocationType, l.OwnerId, l.OwnerType, l.CreatedAt);
}
