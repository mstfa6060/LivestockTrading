using FluentValidation.TestHelper;
using LivestockTrading.Catalog.Application.Features.Brands;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Brands;

public class RejectBrandValidatorTests
{
    private readonly RejectBrandValidator _validator = new();

    private static RejectBrandCommand Cmd(Guid brandId, string reason, Guid actor) =>
        new(brandId, reason, actor);

    [Fact]
    public void Validate_WhenBrandIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(Guid.Empty, "Duplicate brand", Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.BrandId);
    }

    [Fact]
    public void Validate_WhenReasonEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(Guid.NewGuid(), "", Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Reason);
    }

    [Fact]
    public void Validate_WhenReasonTooLong_Fails()
    {
        var result = _validator.TestValidate(Cmd(Guid.NewGuid(), new string('x', 501), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(c => c.Reason);
    }

    [Fact]
    public void Validate_WhenActorAdminIdEmpty_Fails()
    {
        var result = _validator.TestValidate(Cmd(Guid.NewGuid(), "Duplicate brand", Guid.Empty));
        result.ShouldHaveValidationErrorFor(c => c.ActorAdminId);
    }

    [Fact]
    public void Validate_WhenAllValid_Passes()
    {
        var result = _validator.TestValidate(Cmd(Guid.NewGuid(), "Duplicate brand", Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }
}
