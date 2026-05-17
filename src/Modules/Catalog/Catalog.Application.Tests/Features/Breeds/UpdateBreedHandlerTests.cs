using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Breeds;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Breeds;

public class UpdateBreedHandlerTests
{
    private readonly IBreedRepository _repo = Substitute.For<IBreedRepository>();
    private readonly UpdateBreedHandler _handler;

    public UpdateBreedHandlerTests() => _handler = new UpdateBreedHandler(_repo);

    private static ConsumeContext<UpdateBreedCommand> Context(UpdateBreedCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<UpdateBreedCommand>>();
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
    public async Task Consume_WhenBreedExists_Updates_RespondsSuccess()
    {
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleBreed());
        var dto = new UpdateBreedDto(BreedTestHelpers.EnglishText("New"), null, 5);
        var ctx = Context(new UpdateBreedCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenBreedNotFound_RespondsFailure_NotFound()
    {
        _repo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Breed?)null);
        var dto = new UpdateBreedDto(BreedTestHelpers.EnglishText("New"), null, 0);
        var ctx = Context(new UpdateBreedCommand(99, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_BREED"));
    }

    [Fact]
    public async Task Consume_WhenDomainExceptionThrown_RespondsFailure_RuleViolation()
    {
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleBreed());
        // Name without 'en' → UpdateTranslations throws DomainException (Wave 1 guard)
        var dto = new UpdateBreedDto(Shared.ValueObjects.Translations.Empty, null, 0);
        var ctx = Context(new UpdateBreedCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "BREED_RULE_VIOLATION"));
    }
}
