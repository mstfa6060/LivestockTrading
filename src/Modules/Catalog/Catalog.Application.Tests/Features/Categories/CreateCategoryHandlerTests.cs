using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Categories;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

public class CreateCategoryHandlerTests
{
    private readonly ICategoryRepository _repo = Substitute.For<ICategoryRepository>();
    private readonly CreateCategoryHandler _handler;

    public CreateCategoryHandlerTests() => _handler = new CreateCategoryHandler(_repo);

    private static ConsumeContext<CreateCategoryCommand> Context(CreateCategoryCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<CreateCategoryCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_WhenParentCodeNull_CreatesTopLevel_RespondsSuccess()
    {
        var dto = new CreateCategoryDto("CAT1", null, CategoryTestHelpers.EnglishText("Cattle"), null, 0, null);
        var ctx = Context(new CreateCategoryCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(Arg.Is<Result<int>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenParentCodeValid_CreatesSubcategory_RespondsSuccess()
    {
        var parent = Category.CreateTopLevel("PARENT", CategoryTestHelpers.EnglishText("Parent"));
        _repo.GetByCodeAsync("PARENT", Arg.Any<CancellationToken>()).Returns(parent);
        var dto = new CreateCategoryDto("CHILD", "PARENT", CategoryTestHelpers.EnglishText("Child"), null, 0, null);
        var ctx = Context(new CreateCategoryCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(Arg.Is<Result<int>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenParentNotFound_RespondsFailure_NotFoundParent()
    {
        _repo.GetByCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Category?)null);
        var dto = new CreateCategoryDto("CHILD", "MISSING", CategoryTestHelpers.EnglishText("Child"), null, 0, null);
        var ctx = Context(new CreateCategoryCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.DidNotReceive().AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_PARENT_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenDomainExceptionThrown_RespondsFailure_RuleViolation()
    {
        // Empty Code → Category.CreateTopLevel throws DomainException (Wave 1 guard)
        var dto = new CreateCategoryDto("", null, CategoryTestHelpers.EnglishText("X"), null, 0, null);
        var ctx = Context(new CreateCategoryCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "CATEGORY_RULE_VIOLATION"));
    }
}
