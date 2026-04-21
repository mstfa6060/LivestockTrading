using FastEndpoints;
using Livestock.Domain.Entities;
using Livestock.Domain.Errors;
using Livestock.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Features.Admin.PaymentMethods;

public class ListPaymentMethodsEndpoint(LivestockDbContext db) : EndpointWithoutRequest<List<PaymentMethodItem>>
{
    public override void Configure()
    {
        Get("/PaymentMethods");
        AllowAnonymous();
        Tags("PaymentMethods");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var methods = await db.PaymentMethods
            .AsNoTracking()
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.SortOrder).ThenBy(m => m.Name)
            .Select(m => new PaymentMethodItem(m.Id, m.Name, m.Code, m.Description, m.IconUrl, m.IsActive, m.SortOrder, m.CreatedAt))
            .ToListAsync(ct);

        await SendAsync(methods, 200, ct);
    }
}

public class GetPaymentMethodEndpoint(LivestockDbContext db) : Endpoint<GetPaymentMethodRequest, PaymentMethodItem>
{
    public override void Configure()
    {
        Get("/PaymentMethods/{Id}");
        AllowAnonymous();
        Tags("PaymentMethods");
    }

    public override async Task HandleAsync(GetPaymentMethodRequest req, CancellationToken ct)
    {
        var m = await db.PaymentMethods.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.Id && !x.IsDeleted, ct);
        if (m is null)
        {
            AddError(LivestockErrors.PaymentMethodErrors.PaymentMethodNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        await SendAsync(new PaymentMethodItem(m.Id, m.Name, m.Code, m.Description, m.IconUrl, m.IsActive, m.SortOrder, m.CreatedAt), 200, ct);
    }
}

public class CreatePaymentMethodEndpoint(LivestockDbContext db) : Endpoint<CreatePaymentMethodRequest, PaymentMethodItem>
{
    public override void Configure()
    {
        Post("/Admin/PaymentMethods");
        Roles("LivestockTrading.Admin");
        Tags("PaymentMethods");
    }

    public override async Task HandleAsync(CreatePaymentMethodRequest req, CancellationToken ct)
    {
        var exists = await db.PaymentMethods.AnyAsync(m => m.Code == req.Code && !m.IsDeleted, ct);
        if (exists)
        {
            AddError(LivestockErrors.PaymentMethodErrors.PaymentMethodCodeAlreadyExists);
            await SendErrorsAsync(409, ct);
            return;
        }

        var method = new PaymentMethod
        {
            Name = req.Name,
            Code = req.Code.ToUpperInvariant(),
            Description = req.Description,
            IconUrl = req.IconUrl,
            IsActive = req.IsActive,
            SortOrder = req.SortOrder
        };

        db.PaymentMethods.Add(method);
        await db.SaveChangesAsync(ct);

        await SendAsync(new PaymentMethodItem(method.Id, method.Name, method.Code, method.Description, method.IconUrl, method.IsActive, method.SortOrder, method.CreatedAt), 201, ct);
    }
}

public class UpdatePaymentMethodEndpoint(LivestockDbContext db) : Endpoint<UpdatePaymentMethodRequest, PaymentMethodItem>
{
    public override void Configure()
    {
        Put("/Admin/PaymentMethods/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("PaymentMethods");
    }

    public override async Task HandleAsync(UpdatePaymentMethodRequest req, CancellationToken ct)
    {
        var method = await db.PaymentMethods.FirstOrDefaultAsync(m => m.Id == req.Id && !m.IsDeleted, ct);
        if (method is null)
        {
            AddError(LivestockErrors.PaymentMethodErrors.PaymentMethodNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        method.Name = req.Name;
        method.Description = req.Description;
        method.IconUrl = req.IconUrl;
        method.IsActive = req.IsActive;
        method.SortOrder = req.SortOrder;
        method.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await SendAsync(new PaymentMethodItem(method.Id, method.Name, method.Code, method.Description, method.IconUrl, method.IsActive, method.SortOrder, method.CreatedAt), 200, ct);
    }
}

public class DeletePaymentMethodEndpoint(LivestockDbContext db) : Endpoint<DeletePaymentMethodRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("/Admin/PaymentMethods/{Id}");
        Roles("LivestockTrading.Admin");
        Tags("PaymentMethods");
    }

    public override async Task HandleAsync(DeletePaymentMethodRequest req, CancellationToken ct)
    {
        var method = await db.PaymentMethods.FirstOrDefaultAsync(m => m.Id == req.Id && !m.IsDeleted, ct);
        if (method is null)
        {
            AddError(LivestockErrors.PaymentMethodErrors.PaymentMethodNotFound);
            await SendErrorsAsync(404, ct);
            return;
        }

        method.IsDeleted = true;
        method.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
