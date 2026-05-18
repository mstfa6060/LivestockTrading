using LivestockTrading.Catalog.Application.Features.Currencies;
using MassTransit;
using NSubstitute;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Currencies;

public class RefreshExchangeRatesHandlerTests
{
    private readonly RefreshExchangeRatesHandler _handler = new();

    private static ConsumeContext<RefreshExchangeRatesCommand> Context(RefreshExchangeRatesCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<RefreshExchangeRatesCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    // W2.4 NotImpl stub — handler-direct assert (endpoint→mediator fault davranışı Wave 3
    // host-wire scope; B.0.1 notu). Wave 2'de NotImpl-direct test ilk emsal (KAYDET adayı).
    [Fact]
    public async Task Consume_Throws_NotImplementedException()
    {
        var ctx = Context(new RefreshExchangeRatesCommand(Guid.NewGuid()));

        await Assert.ThrowsAsync<NotImplementedException>(() => _handler.Consume(ctx));
    }
}
