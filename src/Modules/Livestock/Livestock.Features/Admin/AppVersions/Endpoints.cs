using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.AppVersions;

public class ListAppVersionsEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<AppVersionItem>>
{
    public override void Configure()
    {
        Get("/Admin/AppVersions");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var versions = await db.AppVersionConfigs
            .AsNoTracking()
            .OrderBy(v => v.Platform)
            .Select(v => new AppVersionItem(v.Id, v.Platform, v.MinSupportedVersion, v.LatestVersion, v.StoreUrl, v.UpdateMessage, v.IsActive, v.CreatedAt, v.UpdatedAt))
            .ToListAsync(ct);

        await SendAsync(versions, 200, ct);
    }
}

public class GetAppVersionEndpoint(LivestockDbContext db) : Endpoint<GetAppVersionRequest, AppVersionItem>
{
    public override void Configure()
    {
        Get("/Admin/AppVersions/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(GetAppVersionRequest req, CancellationToken ct)
    {
        var v = await db.AppVersionConfigs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (v is null)
        {
            AddError(LivestockErrors.AppVersionErrors.AppVersionNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new AppVersionItem(v.Id, v.Platform, v.MinSupportedVersion, v.LatestVersion, v.StoreUrl, v.UpdateMessage, v.IsActive, v.CreatedAt, v.UpdatedAt), 200, ct);
    }
}

public class CreateAppVersionEndpoint(LivestockDbContext db) : Endpoint<CreateAppVersionRequest, AppVersionItem>
{
    public override void Configure()
    {
        Post("/Admin/AppVersions");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(CreateAppVersionRequest req, CancellationToken ct)
    {
        var exists = await db.AppVersionConfigs.AnyAsync(v => v.Platform == req.Platform, ct);
        if (exists)
        {
            AddError(LivestockErrors.AppVersionErrors.AppVersionAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var config = new AppVersionConfig
        {
            Platform = req.Platform,
            MinSupportedVersion = req.MinSupportedVersion,
            LatestVersion = req.LatestVersion,
            StoreUrl = req.StoreUrl,
            UpdateMessage = req.UpdateMessage
        };

        db.AppVersionConfigs.Add(config);
        await db.SaveChangesAsync(ct);

        await SendAsync(new AppVersionItem(config.Id, config.Platform, config.MinSupportedVersion, config.LatestVersion, config.StoreUrl, config.UpdateMessage, config.IsActive, config.CreatedAt, config.UpdatedAt), 201, ct);
    }
}

public class UpdateAppVersionEndpoint(LivestockDbContext db) : Endpoint<UpdateAppVersionRequest, AppVersionItem>
{
    public override void Configure()
    {
        Put("/Admin/AppVersions/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(UpdateAppVersionRequest req, CancellationToken ct)
    {
        var config = await db.AppVersionConfigs.FirstOrDefaultAsync(v => v.Id == req.Id, ct);
        if (config is null)
        {
            AddError(LivestockErrors.AppVersionErrors.AppVersionNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        config.MinSupportedVersion = req.MinSupportedVersion;
        config.LatestVersion = req.LatestVersion;
        config.StoreUrl = req.StoreUrl;
        config.UpdateMessage = req.UpdateMessage;
        config.IsActive = req.IsActive;
        config.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new AppVersionItem(config.Id, config.Platform, config.MinSupportedVersion, config.LatestVersion, config.StoreUrl, config.UpdateMessage, config.IsActive, config.CreatedAt, config.UpdatedAt), 200, ct);
    }
}

public class DeleteAppVersionEndpoint(LivestockDbContext db) : Endpoint<DeleteAppVersionRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/AppVersions/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("Admin");
    }

    public override async Task HandleAsync(DeleteAppVersionRequest req, CancellationToken ct)
    {
        var config = await db.AppVersionConfigs.FirstOrDefaultAsync(v => v.Id == req.Id, ct);
        if (config is null)
        {
            AddError(LivestockErrors.AppVersionErrors.AppVersionNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        db.AppVersionConfigs.Remove(config);
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}

public class CheckAppVersionEndpoint(LivestockDbContext db) : Endpoint<CheckAppVersionRequest, AppVersionCheckResult>
{
    public override void Configure()
    {
        Get("/AppVersions/Check");
        AllowAnonymous();
        Tags("AppVersions");
    }

    public override async Task HandleAsync(CheckAppVersionRequest req, CancellationToken ct)
    {
        var config = await db.AppVersionConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Platform == req.Platform && v.IsActive, ct);

        if (config is null)
        {
            AddError(LivestockErrors.AppVersionErrors.AppVersionNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new AppVersionCheckResult(config.MinSupportedVersion, config.LatestVersion, config.StoreUrl, config.UpdateMessage, config.IsActive), 200, ct);
    }
}
