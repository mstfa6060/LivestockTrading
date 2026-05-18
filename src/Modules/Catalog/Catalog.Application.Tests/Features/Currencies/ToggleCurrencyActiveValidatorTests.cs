using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Currencies;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Currencies;

public class ToggleCurrencyActiveValidatorTests
{
    private readonly ToggleCurrencyActiveValidator _validator = new();

    private static ToggleCurrencyActiveCommand Cmd(int currencyId, Guid actor) =>
        new(currencyId, true, actor);

    [Fact]
    public void Validate_WhenCurrencyIdNotPositive_Fails()
    {
        var result = _validator.TestValidate(Cmd(0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.CurrencyId);
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
