using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Brands;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Brands;

public class CreateBrandValidatorTests
{
    private readonly CreateBrandValidator _validator = new();

    private static CreateBrandCommand Cmd(
        string slug, Translations name, IReadOnlyList<string> categoryCodes, int displayOrder, Guid actor) =>
        new(new CreateBrandDto(slug, name, null, categoryCodes, null, null, null, displayOrder), actor);

    [Fact]
    public void Validate_WhenSlugEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("", BrandTestHelpers.EnglishText("Name"), new[] { "cat1" }, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Slug);
    }

    [Fact]
    public void Validate_WhenNameMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("acme", Translations.Empty, new[] { "cat1" }, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Name);
    }

    [Fact]
    public void Validate_WhenCategoryCodesEmptyList_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("acme", BrandTestHelpers.EnglishText("Name"), Array.Empty<string>(), 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.CategoryCodes);
    }

    [Fact]
    public void Validate_WhenCategoryCodeItemEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("acme", BrandTestHelpers.EnglishText("Name"), new[] { "" }, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor("Dto.CategoryCodes[0]");
    }

    [Fact]
    public void Validate_WhenDisplayOrderNegative_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("acme", BrandTestHelpers.EnglishText("Name"), new[] { "cat1" }, -1, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.DisplayOrder);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("acme", BrandTestHelpers.EnglishText("Name"), new[] { "cat1" }, 0, Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(
            Cmd("acme", BrandTestHelpers.EnglishText("Name"), new[] { "cat1" }, 0, Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
