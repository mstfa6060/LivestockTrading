using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Brands;

/// <summary>
/// Reject endpoint request body. DV4: Contracts'a port amend YAPILMAZ — vertical-slice
/// içinde dosya-içi local record (W2.1/W2.2'de emsal yok, ilk; reason tek alanlık body).
/// </summary>
public sealed record RejectBrandRequest(string Reason);

public static class RejectBrandEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RejectBrandRequest request,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<RejectBrandCommand>();

        // Karar W2.0-3 + Plan-4 doc-grounded: void admin için tek-response yeterli
        // Hem handler hem ValidationFilter aynı tip (Result non-generic) döndürür
        var response = await client.GetResponse<Result>(
            new RejectBrandCommand(id, request.Reason, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
