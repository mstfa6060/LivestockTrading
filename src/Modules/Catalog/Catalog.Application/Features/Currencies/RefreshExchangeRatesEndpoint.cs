using LivestockTrading.Catalog.Application.Common;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Currencies;

public static class RefreshExchangeRatesEndpoint
{
    public static async Task<IResult> Handle(
        IScopedMediator mediator,
        HttpContext http,
        CancellationToken ct)
    {
        var actorAdminId = http.GetActorAdminId();
        var client = mediator.CreateRequestClient<RefreshExchangeRatesCommand>();

        // Handler NotImplementedException fırlatır (Wave 3 Catalog.Infrastructure.RateProviders).
        // Host-inert W2.4: MassTransit fault davranışı Wave 3 host-wire'da netleşir (B.0.1 notu).
        var response = await client.GetResponse<Result>(
            new RefreshExchangeRatesCommand(actorAdminId), ct);

        return response.Message.ToApiResult();
    }
}
