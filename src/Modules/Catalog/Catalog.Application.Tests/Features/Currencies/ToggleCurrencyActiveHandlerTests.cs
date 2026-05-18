using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Currencies;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Currencies;

public class ToggleCurrencyActiveHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly ToggleCurrencyActiveHandler _handler;

    public ToggleCurrencyActiveHandlerTests() => _handler = new ToggleCurrencyActiveHandler(_repo);

    private static ConsumeContext<ToggleCurrencyActiveCommand> Context(ToggleCurrencyActiveCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<ToggleCurrencyActiveCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Currency SampleCurrency() =>
        new("TRY", "Turkish Lira", "TL", 'L', 2, '.', ',');

    [Fact]
    public async Task Consume_WhenActivate_RespondsSuccess()
    {
        _repo.GetCurrencyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleCurrency());
        var ctx = Context(new ToggleCurrencyActiveCommand(1, true, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenDeactivate_RespondsSuccess()
    {
        _repo.GetCurrencyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleCurrency());
        var ctx = Context(new ToggleCurrencyActiveCommand(1, false, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCurrencyNotFound_RespondsFailure_NotFound()
    {
        _repo.GetCurrencyByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Currency?)null);
        var ctx = Context(new ToggleCurrencyActiveCommand(99, true, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CURRENCY"));
    }
}
