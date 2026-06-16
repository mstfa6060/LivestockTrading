using Livestock.Workers.Services.PriceConversion;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using Shared.Contracts.Events.Livestock;
using Shared.Infrastructure.Messaging;

namespace Livestock.Workers.Consumers;

public sealed class PriceConversionConsumer(
    INatsClient nats,
    ICurrencyConverter converter,
    ILogger<PriceConversionConsumer> logger) : NatsConsumerBase<ProductCreatedEvent>(nats)
{
    protected override string Subject => ProductCreatedEvent.Subject; // "livestock.product.created"

    protected override async Task HandleAsync(ProductCreatedEvent message, CancellationToken ct)
    {
        if (message.CurrencyCode == "USD")
        {
            return;
        }

        var usdPrice = await converter.ConvertToUsdAsync(message.Price, message.CurrencyCode, ct);
        logger.LogInformation("Product {ProductId} price: {Price} {Currency} = {UsdPrice} USD", message.ProductId, message.Price, message.CurrencyCode, usdPrice);
    }
}
