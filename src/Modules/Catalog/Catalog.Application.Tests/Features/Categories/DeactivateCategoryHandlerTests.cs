using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Categories;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

public class DeactivateCategoryHandlerTests
{
    private readonly ICategoryRepository _repo = Substitute.For<ICategoryRepository>();
    private readonly DeactivateCategoryHandler _handler;

    public DeactivateCategoryHandlerTests() => _handler = new DeactivateCategoryHandler(_repo);

    private static ConsumeContext<DeactivateCategoryCommand> Context(DeactivateCategoryCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<DeactivateCategoryCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_WhenCategoryExists_Deactivates_RespondsSuccess()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Cattle"));
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        var ctx = Context(new DeactivateCategoryCommand(1, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCategoryNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Category?)null);
        var ctx = Context(new DeactivateCategoryCommand(99, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenAlreadyDeactivated_IsIdempotent_RespondsSuccess()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Cattle"));
        category.Deactivate(); // already inactive
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        var ctx = Context(new DeactivateCategoryCommand(1, Guid.NewGuid()));

        await _handler.Consume(ctx); // idempotent — no throw, success again

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }
}
