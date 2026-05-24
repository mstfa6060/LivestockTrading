using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Currencies;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Currencies;

/// <summary>
/// W3.6.D D.4 revize: NotImpl assert kaldirildi (W2.4-D #161/165 backlog Wave 3 W3.6.D kapanis),
/// ICurrencyRateRefresher delege davranisi guard'a cevrildi (Success-path + Failure-path).
/// Handler sorumlulugu yalniz: Refresher cagir + Result'i RespondAsync. Pattern emsal:
/// ToggleCurrencyActiveHandlerTests (NSubstitute + ConsumeContext direct mock + Received(1) verify).
/// </summary>
public class RefreshExchangeRatesHandlerTests
{
    private readonly ICurrencyRateRefresher _refresher = Substitute.For<ICurrencyRateRefresher>();
    private readonly RefreshExchangeRatesHandler _handler;

    public RefreshExchangeRatesHandlerTests() => _handler = new RefreshExchangeRatesHandler(_refresher);

    private static ConsumeContext<RefreshExchangeRatesCommand> Context(RefreshExchangeRatesCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<RefreshExchangeRatesCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_WhenRefresherSucceeds_RespondsSuccess()
    {
        _refresher.RefreshAsync(Arg.Any<CancellationToken>()).Returns(Result.Success());
        var ctx = Context(new RefreshExchangeRatesCommand(Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _refresher.Received(1).RefreshAsync(Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenRefresherFails_RespondsFailure_AllProvidersFailed()
    {
        var error = new Error("RATE_REFRESH_ALL_PROVIDERS_FAILED", "All 3 currency rate providers failed");
        _refresher.RefreshAsync(Arg.Any<CancellationToken>()).Returns(Result.Failure(error));
        var ctx = Context(new RefreshExchangeRatesCommand(Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _refresher.Received(1).RefreshAsync(Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "RATE_REFRESH_ALL_PROVIDERS_FAILED"));
    }
}
