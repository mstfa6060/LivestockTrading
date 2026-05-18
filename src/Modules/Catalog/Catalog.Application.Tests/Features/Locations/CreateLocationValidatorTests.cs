using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Locations;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Locations;

public class CreateLocationValidatorTests
{
    private readonly CreateLocationValidator _validator = new();

    private static CreateLocationCommand Cmd(
        string code, Shared.Contracts.Catalog.LocationLevel level, string countryCode,
        Translations name, int population, int displayOrder, Guid actor) =>
        new(new CreateLocationDto(null, level, countryCode, code, name, null, population, displayOrder), actor);

    [Fact]
    public void Validate_WhenCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "", Shared.Contracts.Catalog.LocationLevel.Country, "tr",
            LocationTestHelpers.EnglishText("Name"), 0, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Code);
    }

    [Fact]
    public void Validate_WhenCountryCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", Shared.Contracts.Catalog.LocationLevel.Country, "",
            LocationTestHelpers.EnglishText("Name"), 0, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.CountryCode);
    }

    [Fact]
    public void Validate_WhenNameMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", Shared.Contracts.Catalog.LocationLevel.Country, "tr",
            Translations.Empty, 0, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Name);
    }

    [Fact]
    public void Validate_WhenPopulationNegative_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", Shared.Contracts.Catalog.LocationLevel.Country, "tr",
            LocationTestHelpers.EnglishText("Name"), -1, 0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Population);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", Shared.Contracts.Catalog.LocationLevel.Country, "tr",
            LocationTestHelpers.EnglishText("Name"), 0, 0, Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", Shared.Contracts.Catalog.LocationLevel.Country, "tr",
            LocationTestHelpers.EnglishText("Name"), 0, 0, Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
