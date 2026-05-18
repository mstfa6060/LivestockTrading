using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Languages;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Languages;

public class ToggleLanguageActiveValidatorTests
{
    private readonly ToggleLanguageActiveValidator _validator = new();

    private static ToggleLanguageActiveCommand Cmd(int languageId, Guid actor) =>
        new(languageId, true, actor);

    [Fact]
    public void Validate_WhenLanguageIdNotPositive_Fails()
    {
        var result = _validator.TestValidate(Cmd(0, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.LanguageId);
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
