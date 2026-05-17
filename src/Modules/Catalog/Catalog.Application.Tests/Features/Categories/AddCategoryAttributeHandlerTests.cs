using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Categories;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

public class AddCategoryAttributeHandlerTests
{
    private readonly ICategoryRepository _repo = Substitute.For<ICategoryRepository>();
    private readonly AddCategoryAttributeHandler _handler;

    public AddCategoryAttributeHandlerTests() => _handler = new AddCategoryAttributeHandler(_repo);

    private static ConsumeContext<AddCategoryAttributeCommand> Context(AddCategoryAttributeCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<AddCategoryAttributeCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static AttributeDto ValidDto() => new(
        "weight", AttributeValueType.Number, true, true, "kg", null,
        CategoryTestHelpers.EnglishText("Weight"), null, 0);

    [Fact]
    public async Task Consume_WhenCategoryExists_AddsAttribute_RespondsSuccess()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Cattle"));
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        var ctx = Context(new AddCategoryAttributeCommand(1, ValidDto(), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCategoryNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Category?)null);
        var ctx = Context(new AddCategoryAttributeCommand(99, ValidDto(), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenDomainExceptionThrown_RespondsFailure_RuleViolation()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Cattle"));
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        // Empty key → Category.AddAttribute throws DomainException (Wave 1 guard)
        var dto = new AttributeDto("", AttributeValueType.Text, false, false, null, null,
            CategoryTestHelpers.EnglishText("X"), null, 0);
        var ctx = Context(new AddCategoryAttributeCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "CATEGORY_RULE_VIOLATION"));
    }
}
