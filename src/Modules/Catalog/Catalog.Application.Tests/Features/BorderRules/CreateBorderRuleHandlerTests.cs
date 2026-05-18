using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.BorderRules;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Xunit;
using ContractsKind = Shared.Contracts.Catalog.BorderRuleKind;

namespace LivestockTrading.Catalog.Application.Tests.Features.BorderRules;

public class CreateBorderRuleHandlerTests
{
    private readonly IBorderRuleRepository _borderRuleRepo = Substitute.For<IBorderRuleRepository>();
    private readonly ICategoryRepository _categoryRepo = Substitute.For<ICategoryRepository>();
    private readonly IBreedRepository _breedRepo = Substitute.For<IBreedRepository>();
    private readonly CreateBorderRuleHandler _handler;

    public CreateBorderRuleHandlerTests() =>
        _handler = new CreateBorderRuleHandler(_borderRuleRepo, _categoryRepo, _breedRepo);

    private static ConsumeContext<CreateBorderRuleCommand> Context(CreateBorderRuleCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<CreateBorderRuleCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Category Level2Category(string code) =>
        Category.CreateSubcategory(code,
            Category.CreateTopLevel("PARENT", BorderRuleTestHelpers.EnglishText("Parent")),
            BorderRuleTestHelpers.EnglishText(code));

    private static CreateBorderRuleDto Dto(
        string from, string to, string? categoryCode, string? breedCode) =>
        new(from, to, categoryCode, breedCode, ContractsKind.Banned, "{}",
            BorderRuleTestHelpers.EnglishText("note"), null, null);

    [Fact]
    public async Task Consume_WhenRootRule_NoCodeResolution_CreatesBorderRule_RespondsSuccess()
    {
        var ctx = Context(new CreateBorderRuleCommand(Dto("tr", "de", null, null), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _borderRuleRepo.Received(1).AddAsync(Arg.Any<BorderRule>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(Arg.Is<Result<Guid>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCategoryResolved_CreatesBorderRule_RespondsSuccess()
    {
        _categoryRepo.GetByCodeAsync("cattle", Arg.Any<CancellationToken>())
            .Returns(Level2Category("cattle"));
        var ctx = Context(new CreateBorderRuleCommand(Dto("tr", "de", "cattle", null), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _borderRuleRepo.Received(1).AddAsync(Arg.Any<BorderRule>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(Arg.Is<Result<Guid>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenBreedResolved_CreatesBorderRule_RespondsSuccess()
    {
        _breedRepo.GetByCodeAsync("holstein", Arg.Any<CancellationToken>())
            .Returns(Breed.Create("holstein", Level2Category("cattle"), BorderRuleTestHelpers.EnglishText("Holstein")));
        var ctx = Context(new CreateBorderRuleCommand(Dto("tr", "de", null, "holstein"), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _borderRuleRepo.Received(1).AddAsync(Arg.Any<BorderRule>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(Arg.Is<Result<Guid>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCategoryNotFound_RespondsFailure_NotFoundCategory()
    {
        _categoryRepo.GetByCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Category?)null);
        var ctx = Context(new CreateBorderRuleCommand(Dto("tr", "de", "missing", null), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _borderRuleRepo.DidNotReceive().AddAsync(Arg.Any<BorderRule>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(
            Arg.Is<Result<Guid>>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CATEGORY_FOR_BORDER_RULE"));
    }

    [Fact]
    public async Task Consume_WhenBreedNotFound_RespondsFailure_NotFoundBreed()
    {
        _breedRepo.GetByCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Breed?)null);
        var ctx = Context(new CreateBorderRuleCommand(Dto("tr", "de", null, "missing"), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _borderRuleRepo.DidNotReceive().AddAsync(Arg.Any<BorderRule>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(
            Arg.Is<Result<Guid>>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_BREED_FOR_BORDER_RULE"));
    }

    [Fact]
    public async Task Consume_WhenInvalidCountryCode_RespondsFailure_RuleViolation()
    {
        // "X" (1-char) → CountryCode ctor DomainException → BORDER_RULE_VIOLATION
        var ctx = Context(new CreateBorderRuleCommand(Dto("X", "de", null, null), Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _borderRuleRepo.DidNotReceive().AddAsync(Arg.Any<BorderRule>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(
            Arg.Is<Result<Guid>>(r => r.IsFailure && r.Error!.Code == "BORDER_RULE_VIOLATION"));
    }

    [Fact]
    public async Task Consume_WhenNotesMissingEnLocale_RespondsFailure_RuleViolation()
    {
        // BorderRule.Create notes 'en'-guard → DomainException → BORDER_RULE_VIOLATION
        var dto = new CreateBorderRuleDto("tr", "de", null, null, ContractsKind.Banned, "{}",
            Shared.ValueObjects.Translations.Empty, null, null);
        var ctx = Context(new CreateBorderRuleCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _borderRuleRepo.DidNotReceive().AddAsync(Arg.Any<BorderRule>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(
            Arg.Is<Result<Guid>>(r => r.IsFailure && r.Error!.Code == "BORDER_RULE_VIOLATION"));
    }
}
