using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Countries;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Countries;

public class ToggleCountryActiveValidatorTests
{
    private readonly ToggleCountryActiveValidator _validator = new();

    private static ToggleCountryActiveCommand Cmd(int countryId, Guid actor) =>
        new(countryId, true, actor);

    [Fact]
    public void Validate_WhenCountryIdNotPositive_Fails()
    {
        var result = _validator.TestValidate(Cmd(0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.CountryId);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(1, Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(1, Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
