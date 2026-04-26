using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Auth.Login;

public sealed class LoginValidator : Validator<LoginRequest>
{
    private static readonly string[] OAuthProviders = ["google", "apple", "itunes"];

    public LoginValidator()
    {
        RuleFor(x => x)
            .Must(HaveIdentifier)
            .WithMessage("UserName veya Email zorunludur.");

        When(IsNative, () =>
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.");
        });

        When(IsOAuth, () =>
        {
            RuleFor(x => x.ExternalProviderUserId)
                .NotEmpty().WithMessage("ExternalProviderUserId zorunludur.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("OAuth login için Email zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
        });
    }

    private static bool HaveIdentifier(LoginRequest req)
        => !string.IsNullOrWhiteSpace(req.UserName) || !string.IsNullOrWhiteSpace(req.Email);

    private static bool IsNative(LoginRequest req)
        => string.IsNullOrWhiteSpace(req.Provider)
            || string.Equals(req.Provider, "native", StringComparison.OrdinalIgnoreCase);

    private static bool IsOAuth(LoginRequest req)
        => req.Provider is not null
            && OAuthProviders.Contains(req.Provider, StringComparer.OrdinalIgnoreCase);
}
