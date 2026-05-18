using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Languages;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Languages;

public class ToggleLanguageActiveHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly ToggleLanguageActiveHandler _handler;

    public ToggleLanguageActiveHandlerTests() => _handler = new ToggleLanguageActiveHandler(_repo);

    private static ConsumeContext<ToggleLanguageActiveCommand> Context(ToggleLanguageActiveCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<ToggleLanguageActiveCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Language SampleLanguage() =>
        new("tr", "Turkish", "Turkce", false, 0);

    [Fact]
    public async Task Consume_WhenActivate_RespondsSuccess()
    {
        _repo.GetLanguageByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleLanguage());
        var ctx = Context(new ToggleLanguageActiveCommand(1, true, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenDeactivate_RespondsSuccess()
    {
        _repo.GetLanguageByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleLanguage());
        var ctx = Context(new ToggleLanguageActiveCommand(1, false, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenLanguageNotFound_RespondsFailure_NotFound()
    {
        _repo.GetLanguageByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Language?)null);
        var ctx = Context(new ToggleLanguageActiveCommand(99, true, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_LANGUAGE"));
    }
}
