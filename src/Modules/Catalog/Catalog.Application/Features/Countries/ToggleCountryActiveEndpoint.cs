using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Countries;

/// <summary>
/// Toggle endpoint request body. DV4: Contracts'a port amend YAPILMAZ — vertical-slice
/// içinde dosya-içi local record (W2.3 RejectBrandRequest emsali; Active tek alanlık body).
/// </summary>
public sealed record ToggleActiveRequest(bool Active);

public static class ToggleCountryActiveEndpoint
{
    public static async Task<IResult> Handle(
        int id,
        ToggleActiveRequest request,
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<ToggleCountryActiveCommand>();

        var response = await client.GetResponse<Result>(
            new ToggleCountryActiveCommand(id, request.Active, actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
