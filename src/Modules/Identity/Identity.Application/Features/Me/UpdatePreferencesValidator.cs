using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class UpdatePreferencesValidator : AbstractValidator<UpdatePreferencesCommand>
{
    public UpdatePreferencesValidator()
    {
        RuleFor(x => x.Preferences)
            .NotNull().WithMessage("Preferences is required.");

        // String fields must carry a value — UserPreferences has no Domain VO
        // ctor to guard against empty strings (the record is a cross-module
        // shape, not a Domain VO), so the format gate lives here.
        RuleFor(x => x.Preferences.Locale)
            .NotEmpty().WithMessage("Locale is required.");

        RuleFor(x => x.Preferences.CurrencyCode)
            .NotEmpty().WithMessage("CurrencyCode is required.");

        RuleFor(x => x.Preferences.CountryCode)
            .NotEmpty().WithMessage("CountryCode is required.");

        RuleFor(x => x.Preferences.TimeZone)
            .NotEmpty().WithMessage("TimeZone is required.");
    }
}
