using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Brands;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Brands;

public class RejectBrandHandlerTests
{
    private readonly IBrandRepository _repo = Substitute.For<IBrandRepository>();
    private readonly RejectBrandHandler _handler;

    public RejectBrandHandlerTests() => _handler = new RejectBrandHandler(_repo);

    private static ConsumeContext<RejectBrandCommand> Context(RejectBrandCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<RejectBrandCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_WhenSuggestedBrand_Rejects_RespondsSuccess()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(BrandTestHelpers.SuggestedBrand());
        var ctx = Context(new RejectBrandCommand(Guid.NewGuid(), "Duplicate brand", Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenBrandNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Brand?)null);
        var ctx = Context(new RejectBrandCommand(Guid.NewGuid(), "Duplicate brand", Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_BRAND"));
    }

    [Fact]
    public async Task Consume_WhenNotSuggested_RespondsFailure_RuleViolation()
    {
        // Already Approved → Reject throws DomainException (Status != Suggested) — NOT idempotent
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(BrandTestHelpers.ApprovedBrand());
        var ctx = Context(new RejectBrandCommand(Guid.NewGuid(), "Duplicate brand", Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "BRAND_RULE_VIOLATION"));
    }
}
