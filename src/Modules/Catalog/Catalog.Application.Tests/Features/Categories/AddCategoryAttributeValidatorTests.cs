using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Categories;
using Shared.Contracts.Catalog;
using Shared.Contracts.Catalog.Admin;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Categories;

public class AddCategoryAttributeValidatorTests
{
    private readonly AddCategoryAttributeValidator _validator = new();

    private static AddCategoryAttributeCommand Cmd(AttributeDto dto) =>
        new(1, dto, Guid.NewGuid());

    private static AttributeDto Dto(
        string key, AttributeValueType type, string? optionsJson) =>
        new(key, type, false, false, null, optionsJson,
            CategoryTestHelpers.EnglishText("Label"), null, 0);

    [Fact]
    public void Validate_WhenKeyEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(Dto("", AttributeValueType.Text, null)));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Key);
    }

    [Fact]
    public void Validate_WhenValueTypeEnum_AndOptionsJsonEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(Dto("color", AttributeValueType.Enum, null)));
        result.ShouldHaveValidationErrorFor(c => c.Dto.OptionsJson);
    }

    [Fact]
    public void Validate_WhenValueTypeText_AndOptionsJsonEmpty_Passes()
    {
        var result = _validator.TestValidate(Cmd(Dto("note", AttributeValueType.Text, null)));
        result.ShouldNotHaveValidationErrorFor(c => c.Dto.OptionsJson);
    }

    [Fact]
    public void Validate_WhenLabelMissingEnLocale_Fails()
    {
        var dto = new AttributeDto("k", AttributeValueType.Text, false, false, null, null,
            Shared.ValueObjects.Translations.Empty, null, 0);
        var result = _validator.TestValidate(Cmd(dto));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Label);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(Dto("weight", AttributeValueType.Number, null)));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
