using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Categories;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

public class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator = new();

    private static CreateCategoryCommand Cmd(
        string code, Translations name, int displayOrder, Guid actor) =>
        new(new CreateCategoryDto(code, null, name, null, displayOrder, null), actor);

    [Fact]
    public void Validate_WhenCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("", CategoryTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Code);
    }

    [Fact]
    public void Validate_WhenNameMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("CAT1", Translations.Empty, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Name);
    }

    [Fact]
    public void Validate_WhenDisplayOrderNegative_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("CAT1", CategoryTestHelpers.EnglishText("Name"), -1, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.DisplayOrder);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("CAT1", CategoryTestHelpers.EnglishText("Name"), 0, Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(
            Cmd("CAT1", CategoryTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
