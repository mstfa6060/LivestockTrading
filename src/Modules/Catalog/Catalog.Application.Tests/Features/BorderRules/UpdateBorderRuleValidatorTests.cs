using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.BorderRules;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;
using ContractsKind = Shared.Contracts.Catalog.BorderRuleKind;

namespace LivestockTrading.Catalog.Application.Tests.Features.BorderRules;

public class UpdateBorderRuleValidatorTests
{
    private readonly UpdateBorderRuleValidator _validator = new();

    private static UpdateBorderRuleCommand Cmd(
        Guid id, ContractsKind kind, Translations notes, Guid actor) =>
        new(id, new UpdateBorderRuleDto(kind, "{}", notes, null, null), actor);

    [Fact]
    public void Validate_WhenBorderRuleIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            Guid.Empty, ContractsKind.Banned, BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.BorderRuleId);
    }

    [Fact]
    public void Validate_WhenKindOutOfEnum_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            Guid.NewGuid(), (ContractsKind)999, BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Kind);
    }

    [Fact]
    public void Validate_WhenNotesMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            Guid.NewGuid(), ContractsKind.Banned, Translations.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Notes);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            Guid.NewGuid(), ContractsKind.Banned, BorderRuleTestHelpers.EnglishText("n"), Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(
            Guid.NewGuid(), ContractsKind.Banned, BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
