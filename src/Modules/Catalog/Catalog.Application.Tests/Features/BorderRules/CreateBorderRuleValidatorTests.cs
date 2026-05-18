using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.BorderRules;
using Shared.Contracts.Catalog.Admin;
using Shared.ValueObjects;
using Xunit;
using ContractsKind = Shared.Contracts.Catalog.BorderRuleKind;

namespace LivestockTrading.Catalog.Application.Tests.Features.BorderRules;

public class CreateBorderRuleValidatorTests
{
    private readonly CreateBorderRuleValidator _validator = new();

    private static CreateBorderRuleCommand Cmd(
        string from, string to, ContractsKind kind, string json, Translations notes, Guid actor) =>
        new(new CreateBorderRuleDto(from, to, null, null, kind, json, notes, null, null), actor);

    [Fact]
    public void Validate_WhenFromCountryCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "", "de", ContractsKind.Banned, "{}", BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.FromCountryCode);
    }

    [Fact]
    public void Validate_WhenToCountryCodeEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", "", ContractsKind.Banned, "{}", BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.ToCountryCode);
    }

    [Fact]
    public void Validate_WhenKindOutOfEnum_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", "de", (ContractsKind)999, "{}", BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Kind);
    }

    [Fact]
    public void Validate_WhenRestrictionsJsonEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", "de", ContractsKind.Banned, "", BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.RestrictionsJson);
    }

    [Fact]
    public void Validate_WhenNotesMissingEnLocale_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", "de", ContractsKind.Banned, "{}", Translations.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Dto.Notes);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", "de", ContractsKind.Banned, "{}", BorderRuleTestHelpers.EnglishText("n"), Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(
            "tr", "de", ContractsKind.Banned, "{}", BorderRuleTestHelpers.EnglishText("n"), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
