using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.BorderRules;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.BorderRules;

public class DeactivateBorderRuleHandlerTests
{
    private readonly IBorderRuleRepository _repo = Substitute.For<IBorderRuleRepository>();
    private readonly DeactivateBorderRuleHandler _handler;

    public DeactivateBorderRuleHandlerTests() => _handler = new DeactivateBorderRuleHandler(_repo);

    private static ConsumeContext<DeactivateBorderRuleCommand> Context(DeactivateBorderRuleCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<DeactivateBorderRuleCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_WhenBorderRuleExists_Deactivates_RespondsSuccess()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(BorderRuleTestHelpers.SampleBorderRule());
        var ctx = Context(new DeactivateBorderRuleCommand(Guid.NewGuid(), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenAlreadyInactive_IsIdempotent_RespondsSuccess()
    {
        var rule = BorderRuleTestHelpers.SampleBorderRule();
        rule.Deactivate(Guid.NewGuid()); // already inactive
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(rule);
        var ctx = Context(new DeactivateBorderRuleCommand(Guid.NewGuid(), Guid.NewGuid()));

        await _handler.Consume(ctx); // idempotent — no throw, success again

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenBorderRuleNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((BorderRule?)null);
        var ctx = Context(new DeactivateBorderRuleCommand(Guid.NewGuid(), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_BORDER_RULE"));
    }
}
