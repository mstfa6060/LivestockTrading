using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Breeds;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Breeds;

public class CreateBreedHandlerTests
{
    private readonly IBreedRepository _breedRepo = Substitute.For<IBreedRepository>();
    private readonly ICategoryRepository _categoryRepo = Substitute.For<ICategoryRepository>();
    private readonly CreateBreedHandler _handler;

    public CreateBreedHandlerTests() => _handler = new CreateBreedHandler(_breedRepo, _categoryRepo);

    private static ConsumeContext<CreateBreedCommand> Context(CreateBreedCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<CreateBreedCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Category Level2Category()
    {
        var parent = Category.CreateTopLevel("PARENT", BreedTestHelpers.EnglishText("Parent"));
        return Category.CreateSubcategory("SUBCAT", parent, BreedTestHelpers.EnglishText("Sub"));
    }

    [Fact]
    public async Task Consume_WhenCategoryLevel2_CreatesBreed_RespondsSuccess()
    {
        _categoryRepo.GetByCodeAsync("SUBCAT", Arg.Any<CancellationToken>()).Returns(Level2Category());
        var dto = new CreateBreedDto("holstein", "SUBCAT", null, BreedTestHelpers.EnglishText("Holstein"), null, 0);
        var ctx = Context(new CreateBreedCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _breedRepo.Received(1).AddAsync(Arg.Any<Breed>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(Arg.Is<Result<int>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenParentCategoryNotFound_RespondsFailure_NotFoundParent()
    {
        _categoryRepo.GetByCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Category?)null);
        var dto = new CreateBreedDto("holstein", "MISSING", null, BreedTestHelpers.EnglishText("Holstein"), null, 0);
        var ctx = Context(new CreateBreedCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _breedRepo.DidNotReceive().AddAsync(Arg.Any<Breed>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_PARENT_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenCategoryNotLevel2_RespondsFailure_RuleViolation()
    {
        // Level-1 (top-level) category → Breed.Create throws DomainException (Wave 1 guard: must be level-2)
        var level1 = Category.CreateTopLevel("CAT1", BreedTestHelpers.EnglishText("Cattle"));
        _categoryRepo.GetByCodeAsync("CAT1", Arg.Any<CancellationToken>()).Returns(level1);
        var dto = new CreateBreedDto("holstein", "CAT1", null, BreedTestHelpers.EnglishText("Holstein"), null, 0);
        var ctx = Context(new CreateBreedCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _breedRepo.DidNotReceive().AddAsync(Arg.Any<Breed>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "BREED_RULE_VIOLATION"));
    }
}
