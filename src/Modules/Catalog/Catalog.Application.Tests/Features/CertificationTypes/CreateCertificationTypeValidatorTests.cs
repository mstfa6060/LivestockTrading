using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.CertificationTypes;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.CertificationTypes;

public class CreateCertificationTypeValidatorTests
{
    private readonly CreateCertificationTypeValidator _validator = new();

    private static CreateCertificationTypeCommand Cmd(
        string code, Translations name, int displayOrder, Guid actor) =>
        new(new CreateCertificationTypeDto(code, name, null, displayOrder), actor);

    [Fact]
    public void Validate_WhenCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "", CertificationTypeTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Code);
    }

    [Fact]
    public void Validate_WhenNameMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "cert", Translations.Empty, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.NameTranslations);
    }

    [Fact]
    public void Validate_WhenDisplayOrderNegative_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "cert", CertificationTypeTestHelpers.EnglishText("Name"), -1, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.DisplayOrder);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "cert", CertificationTypeTestHelpers.EnglishText("Name"), 0, Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(
            "cert", CertificationTypeTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
