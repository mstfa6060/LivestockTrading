using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Enums;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Identity;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Features.Transporters;

public class GetAllTransportersEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<TransporterListItem>>
{
    public override void Configure()
    {
        Get("/Transporters");
        AllowAnonymous();
        Tags("Transporters");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var transporters = await db.Transporters
            .AsNoTracking()
            .Where(t => t.Status == TransporterStatus.Active)
            .OrderByDescending(t => t.AverageRating)
            .Select(t => new TransporterListItem(t.Id, t.UserId, t.CompanyName, t.Status, t.AverageRating, t.ReviewCount, t.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(transporters, 200, ct);
    }
}

public class GetTransporterEndpoint(LivestockDbContext db) : Endpoint<GetTransporterRequest, TransporterDetail>
{
    public override void Configure()
    {
        Get("/Transporters/{Id}");
        AllowAnonymous();
        Tags("Transporters");
    }

    public override async Task HandleAsync(GetTransporterRequest req, CancellationToken ct)
    {
        var t = await db.Transporters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (t is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new TransporterDetail(t.Id, t.UserId, t.CompanyName, t.Description, t.PhoneNumber, t.Email, t.WebsiteUrl, t.LicenseNumber, t.LogoUrl, t.Status, t.AverageRating, t.ReviewCount, t.VerifiedAt, t.CreatedAt), 200, ct);
    }
}

public class GetMyTransporterEndpoint(LivestockDbContext db, IUserContext user) : EndpointWithoutRequest<TransporterDetail>
{
    public override void Configure()
    {
        Get("/Transporters/Me");
        Tags("Transporters");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var t = await db.Transporters.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.UserId, ct);
        if (t is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new TransporterDetail(t.Id, t.UserId, t.CompanyName, t.Description, t.PhoneNumber, t.Email, t.WebsiteUrl, t.LicenseNumber, t.LogoUrl, t.Status, t.AverageRating, t.ReviewCount, t.VerifiedAt, t.CreatedAt), 200, ct);
    }
}

public class BecomeTransporterEndpoint(LivestockDbContext db, IUserContext user, IEventPublisher publisher) : Endpoint<BecomeTransporterRequest, TransporterDetail>
{
    public override void Configure()
    {
        Post("/Transporters/Register");
        Tags("Transporters");
    }

    public override async Task HandleAsync(BecomeTransporterRequest req, CancellationToken ct)
    {
        var exists = await db.Transporters.AnyAsync(t => t.UserId == user.UserId, ct);
        if (exists)
        {
            AddError(LivestockErrors.Common.InvalidId);
            await SendErrorsAsync(409, ct);
            return;
        }

        var transporter = new Transporter
        {
            UserId = user.UserId, CompanyName = req.CompanyName, Description = req.Description,
            PhoneNumber = req.PhoneNumber, Email = req.Email, WebsiteUrl = req.WebsiteUrl,
            LicenseNumber = req.LicenseNumber, Status = TransporterStatus.PendingVerification
        };

        db.Transporters.Add(transporter);
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(TransporterRegisteredEvent.Subject, new TransporterRegisteredEvent
        {
            TransporterId = transporter.Id,
            UserId = transporter.UserId,
            CompanyName = transporter.CompanyName
        }, ct);

        await SendAsync(new TransporterDetail(transporter.Id, transporter.UserId, transporter.CompanyName, transporter.Description, transporter.PhoneNumber, transporter.Email, transporter.WebsiteUrl, transporter.LicenseNumber, null, transporter.Status, 0, 0, null, transporter.CreatedAt), 201, ct);
    }
}

public class UpdateTransporterEndpoint(LivestockDbContext db, IUserContext user) : Endpoint<UpdateTransporterRequest, TransporterDetail>
{
    public override void Configure()
    {
        Put("/Transporters/Me");
        Tags("Transporters");
    }

    public override async Task HandleAsync(UpdateTransporterRequest req, CancellationToken ct)
    {
        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.UserId == user.UserId, ct);
        if (transporter is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        transporter.CompanyName = req.CompanyName; transporter.Description = req.Description;
        transporter.PhoneNumber = req.PhoneNumber; transporter.Email = req.Email;
        transporter.WebsiteUrl = req.WebsiteUrl; transporter.LicenseNumber = req.LicenseNumber;
        transporter.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new TransporterDetail(transporter.Id, transporter.UserId, transporter.CompanyName, transporter.Description, transporter.PhoneNumber, transporter.Email, transporter.WebsiteUrl, transporter.LicenseNumber, transporter.LogoUrl, transporter.Status, transporter.AverageRating, transporter.ReviewCount, transporter.VerifiedAt, transporter.CreatedAt), 200, ct);
    }
}

public class VerifyTransporterEndpoint(LivestockDbContext db, IEventPublisher publisher) : Endpoint<VerifyTransporterRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Transporters/{Id}/Verify");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Transporters");
    }

    public override async Task HandleAsync(VerifyTransporterRequest req, CancellationToken ct)
    {
        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.Id == req.Id, ct);
        if (transporter is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (transporter.Status == TransporterStatus.Active)
        {
            AddError(LivestockErrors.TransportErrors.TransporterAlreadyVerified);
            await SendErrorsAsync(409, ct);
            return;
        }

        transporter.Status = TransporterStatus.Active;
        transporter.VerifiedAt = DateTime.UtcNow;
        transporter.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await publisher.PublishAsync(TransporterVerifiedEvent.Subject, new TransporterVerifiedEvent
        {
            TransporterId = transporter.Id,
            UserId = transporter.UserId
        }, ct);

        await SendNoContentAsync(ct);
    }
}

public class SuspendTransporterEndpoint(LivestockDbContext db) : Endpoint<SuspendTransporterRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/Transporters/{Id}/Suspend");
        Roles("LivestockTrading.Admin", "LivestockTrading.Moderator");
        Tags("Transporters");
    }

    public override async Task HandleAsync(SuspendTransporterRequest req, CancellationToken ct)
    {
        var transporter = await db.Transporters.FirstOrDefaultAsync(t => t.Id == req.Id, ct);
        if (transporter is null)
        {
            AddError(LivestockErrors.TransportErrors.TransporterNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        if (transporter.Status == TransporterStatus.Suspended)
        {
            AddError(LivestockErrors.TransportErrors.TransporterAlreadySuspended);
            await SendErrorsAsync(409, ct);
            return;
        }

        transporter.Status = TransporterStatus.Suspended;
        transporter.SuspensionReason = req.Reason;
        transporter.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
