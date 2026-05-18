using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.CertificationTypes;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.CertificationTypes;

public class UpdateCertificationTypeValidatorTests
{
    private readonly UpdateCertificationTypeValidator _validator = new();

    private static UpdateCertificationTypeCommand Cmd(
        int id, Translations name, int displayOrder, Guid actor) =>
        new(id, new UpdateCertificationTypeDto(name, null, displayOrder), actor);

    [Fact]
    public void Validate_WhenCertificationTypeIdZero_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            0, CertificationTypeTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.CertificationTypeId);
    }

    [Fact]
    public void Validate_WhenNameMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            1, Translations.Empty, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.NameTranslations);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            1, CertificationTypeTestHelpers.EnglishText("Name"), 0, Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(
            1, CertificationTypeTestHelpers.EnglishText("Name"), 0, Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
