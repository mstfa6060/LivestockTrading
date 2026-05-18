using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Countries;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Countries;

public class ToggleCountryActiveHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly ToggleCountryActiveHandler _handler;

    public ToggleCountryActiveHandlerTests() => _handler = new ToggleCountryActiveHandler(_repo);

    private static ConsumeContext<ToggleCountryActiveCommand> Context(ToggleCountryActiveCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<ToggleCountryActiveCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Country SampleCountry() =>
        new("TR", "Turkey", "Turkiye", "MENA", "TRY", "tr", "+90", 0);

    [Fact]
    public async Task Consume_WhenActivate_RespondsSuccess()
    {
        _repo.GetCountryByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleCountry());
        var ctx = Context(new ToggleCountryActiveCommand(1, true, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenDeactivate_RespondsSuccess()
    {
        _repo.GetCountryByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleCountry());
        var ctx = Context(new ToggleCountryActiveCommand(1, false, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCountryNotFound_RespondsFailure_NotFound()
    {
        _repo.GetCountryByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Country?)null);
        var ctx = Context(new ToggleCountryActiveCommand(99, true, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_COUNTRY"));
    }
}
