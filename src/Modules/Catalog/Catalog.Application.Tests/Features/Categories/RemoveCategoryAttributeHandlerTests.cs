using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Categories;
using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

public class RemoveCategoryAttributeHandlerTests
{
    private readonly ICategoryRepository _repo = Substitute.For<ICategoryRepository>();
    private readonly RemoveCategoryAttributeHandler _handler;

    public RemoveCategoryAttributeHandlerTests() => _handler = new RemoveCategoryAttributeHandler(_repo);

    private static ConsumeContext<RemoveCategoryAttributeCommand> Context(RemoveCategoryAttributeCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<RemoveCategoryAttributeCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_WhenAttributeExists_Removes_RespondsSuccess()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Cattle"));
        var attr = category.AddAttribute("weight", AttributeValueType.Number, true, true,
            "kg", null, CategoryTestHelpers.EnglishText("Weight"), null, 0);
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        var ctx = Context(new RemoveCategoryAttributeCommand(1, attr.Id, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCategoryNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Category?)null);
        var ctx = Context(new RemoveCategoryAttributeCommand(99, Guid.NewGuid(), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenAttributeNotFound_RespondsFailure_RuleViolation()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Cattle"));
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        // Random attributeId not present → Category.RemoveAttribute throws DomainException
        var ctx = Context(new RemoveCategoryAttributeCommand(1, Guid.NewGuid(), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "CATEGORY_RULE_VIOLATION"));
    }
}
