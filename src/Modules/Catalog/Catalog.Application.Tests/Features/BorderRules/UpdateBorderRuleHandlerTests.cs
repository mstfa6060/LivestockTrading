using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.BorderRules;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Shared.ValueObjects;
using Xunit;
using ContractsKind = Shared.Contracts.Catalog.BorderRuleKind;

namespace LivestockTrading.Catalog.Application.Tests.Features.BorderRules;

public class UpdateBorderRuleHandlerTests
{
    private readonly IBorderRuleRepository _repo = Substitute.For<IBorderRuleRepository>();
    private readonly UpdateBorderRuleHandler _handler;

    public UpdateBorderRuleHandlerTests() => _handler = new UpdateBorderRuleHandler(_repo);

    private static ConsumeContext<UpdateBorderRuleCommand> Context(UpdateBorderRuleCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<UpdateBorderRuleCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static UpdateBorderRuleDto Dto(Translations notes) =>
        new(ContractsKind.RequiresCert, "{\"v\":1}", notes, null, null);

    [Fact]
    public async Task Consume_WhenBorderRuleExists_Updates_RespondsSuccess()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(BorderRuleTestHelpers.SampleBorderRule());
        var ctx = Context(new UpdateBorderRuleCommand(
            Guid.NewGuid(), Dto(BorderRuleTestHelpers.EnglishText("upd")), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenBorderRuleNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((BorderRule?)null);
        var ctx = Context(new UpdateBorderRuleCommand(
            Guid.NewGuid(), Dto(BorderRuleTestHelpers.EnglishText("upd")), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_BORDER_RULE"));
    }

    [Fact]
    public async Task Consume_WhenNotesMissingEnLocale_RespondsFailure_RuleViolation()
    {
        // BorderRule.Update notes 'en'-guard (W2.5-A) → DomainException → BORDER_RULE_VIOLATION
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(BorderRuleTestHelpers.SampleBorderRule());
        var ctx = Context(new UpdateBorderRuleCommand(
            Guid.NewGuid(), Dto(Translations.Empty), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "BORDER_RULE_VIOLATION"));
    }
}
