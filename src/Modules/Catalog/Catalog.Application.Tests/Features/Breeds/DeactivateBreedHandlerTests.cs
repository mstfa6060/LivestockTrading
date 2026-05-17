using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Breeds;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Breeds;

public class DeactivateBreedHandlerTests
{
    private readonly IBreedRepository _repo = Substitute.For<IBreedRepository>();
    private readonly DeactivateBreedHandler _handler;

    public DeactivateBreedHandlerTests() => _handler = new DeactivateBreedHandler(_repo);

    private static ConsumeContext<DeactivateBreedCommand> Context(DeactivateBreedCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<DeactivateBreedCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Breed SampleBreed()
    {
        var parent = Category.CreateTopLevel("PARENT", BreedTestHelpers.EnglishText("Parent"));
        var category = Category.CreateSubcategory("SUBCAT", parent, BreedTestHelpers.EnglishText("Sub"));
        return Breed.Create("holstein", category, BreedTestHelpers.EnglishText("Holstein"));
    }

    [Fact]
    public async Task Consume_WhenBreedExists_Deactivates_RespondsSuccess()
    {
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleBreed());
        var ctx = Context(new DeactivateBreedCommand(1, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenBreedNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Breed?)null);
        var ctx = Context(new DeactivateBreedCommand(99, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_BREED"));
    }

    [Fact]
    public async Task Consume_WhenAlreadyDeactivated_IsIdempotent_RespondsSuccess()
    {
        var breed = SampleBreed();
        breed.Deactivate(); // already inactive
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(breed);
        var ctx = Context(new DeactivateBreedCommand(1, Guid.NewGuid()));

        await _handler.Consume(ctx); // idempotent — no throw, success again

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }
}
