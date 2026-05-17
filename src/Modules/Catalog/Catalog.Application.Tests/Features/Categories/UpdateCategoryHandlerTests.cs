using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Categories;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

public class UpdateCategoryHandlerTests
{
    private readonly ICategoryRepository _repo = Substitute.For<ICategoryRepository>();
    private readonly UpdateCategoryHandler _handler;

    public UpdateCategoryHandlerTests() => _handler = new UpdateCategoryHandler(_repo);

    private static ConsumeContext<UpdateCategoryCommand> Context(UpdateCategoryCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<UpdateCategoryCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_WhenCategoryExists_Updates_RespondsSuccess()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Old"));
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        var dto = new UpdateCategoryDto(CategoryTestHelpers.EnglishText("New"), null, 5, "icon");
        var ctx = Context(new UpdateCategoryCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCategoryNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Category?)null);
        var dto = new UpdateCategoryDto(CategoryTestHelpers.EnglishText("New"), null, 0, null);
        var ctx = Context(new UpdateCategoryCommand(99, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenDomainExceptionThrown_RespondsFailure_RuleViolation()
    {
        var category = Category.CreateTopLevel("CAT1", CategoryTestHelpers.EnglishText("Old"));
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        // Name without 'en' → UpdateTranslations throws DomainException (Wave 1 guard)
        var dto = new UpdateCategoryDto(Shared.ValueObjects.Translations.Empty, null, 0, null);
        var ctx = Context(new UpdateCategoryCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "CATEGORY_RULE_VIOLATION"));
    }
}
