using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Brands;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Brands;

public class CreateBrandHandlerTests
{
    private readonly IBrandRepository _brandRepo = Substitute.For<IBrandRepository>();
    private readonly ICategoryRepository _categoryRepo = Substitute.For<ICategoryRepository>();
    private readonly CreateBrandHandler _handler;

    public CreateBrandHandlerTests() => _handler = new CreateBrandHandler(_brandRepo, _categoryRepo);

    private static ConsumeContext<CreateBrandCommand> Context(CreateBrandCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<CreateBrandCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    // Brand'da Category Level invariant YOK (Plan-1/DV1) — W2.2 Level2Category Breed'e özgüydü.
    // Pozitif Id S-A reflection ile (KAYDET-29); BrandCategory ctor categoryId > 0 guard'ı geçer.
    private static Category ResolvedCategory(string code, int id) =>
        Category.CreateTopLevel(code, BrandTestHelpers.EnglishText(code)).WithId(id);

    private static CreateBrandCommand SampleCommand(IReadOnlyList<string> categoryCodes, string? origin = null) =>
        new(new CreateBrandDto("acme", BrandTestHelpers.EnglishText("Acme"), null, categoryCodes,
            null, null, origin, 0), Guid.NewGuid());

    [Fact]
    public async Task Consume_WhenAllCategoriesResolved_CreatesBrand_RespondsSuccess()
    {
        _categoryRepo.GetByCodeAsync("category-1", Arg.Any<CancellationToken>())
            .Returns(ResolvedCategory("category-1", 1));
        _categoryRepo.GetByCodeAsync("category-2", Arg.Any<CancellationToken>())
            .Returns(ResolvedCategory("category-2", 2));
        var ctx = Context(SampleCommand(new[] { "category-1", "category-2" }));

        await _handler.Consume(ctx);

        await _brandRepo.Received(1).AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(Arg.Is<Result<Guid>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenFirstCategoryNotFound_RespondsFailure_NotFoundParent()
    {
        _categoryRepo.GetByCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Category?)null);
        var ctx = Context(SampleCommand(new[] { "missing-1" }));

        await _handler.Consume(ctx);

        await _brandRepo.DidNotReceive().AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(
            Arg.Is<Result<Guid>>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_PARENT_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenSecondCategoryNotFound_FailsFast_NotFoundParent()
    {
        _categoryRepo.GetByCodeAsync("category-1", Arg.Any<CancellationToken>())
            .Returns(ResolvedCategory("category-1", 1));
        _categoryRepo.GetByCodeAsync("missing-2", Arg.Any<CancellationToken>())
            .Returns((Category?)null);
        var ctx = Context(SampleCommand(new[] { "category-1", "missing-2" }));

        await _handler.Consume(ctx);

        // fail-fast: Brand.CreateByAdmin'e ulaşılmadı (BrandCategory ctor patlamadı)
        await _brandRepo.DidNotReceive().AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(
            Arg.Is<Result<Guid>>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_PARENT_CATEGORY"));
    }

    [Fact]
    public async Task Consume_WhenInvalidOriginCountry_RespondsFailure_RuleViolation()
    {
        _categoryRepo.GetByCodeAsync("category-1", Arg.Any<CancellationToken>())
            .Returns(ResolvedCategory("category-1", 1));
        // origin "X" (1-char) → CountryCode ctor DomainException (Brand.CreateByAdmin'den önce)
        var ctx = Context(SampleCommand(new[] { "category-1" }, origin: "X"));

        await _handler.Consume(ctx);

        await _brandRepo.DidNotReceive().AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<Guid>>(
            Arg.Is<Result<Guid>>(r => r.IsFailure && r.Error!.Code == "BRAND_RULE_VIOLATION"));
    }
}
