using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Breeds;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Breeds;

public class CreateBreedValidatorTests
{
    private readonly CreateBreedValidator _validator = new();

    private static CreateBreedCommand Cmd(
        string code, string categoryCode, Translations name, int displayOrder, Guid actor) =>
        new(new CreateBreedDto(code, categoryCode, null, name, null, displayOrder), actor);

    [Fact]
    public void Validate_WhenCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("", "SUBCAT", BreedTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Code);
    }

    [Fact]
    public void Validate_WhenCategoryCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("holstein", "", BreedTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.CategoryCode);
    }

    [Fact]
    public void Validate_WhenNameMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("holstein", "SUBCAT", Translations.Empty, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Name);
    }

    [Fact]
    public void Validate_WhenDisplayOrderNegative_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("holstein", "SUBCAT", BreedTestHelpers.EnglishText("Name"), -1, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.DisplayOrder);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(
            Cmd("holstein", "SUBCAT", BreedTestHelpers.EnglishText("Name"), 0, Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(
            Cmd("holstein", "SUBCAT", BreedTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
