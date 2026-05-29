using Shared.Results;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for validating an OAuth identity token issued by an external
/// provider (Google, Apple). Concrete implementation lives in Identity.Infrastructure
/// (Wave 4 W4.3) as a Faz 1 NoOp + log adapter; the real provider integration
/// tracks against Backlog #79 (Apple Sign In key management `.p8` storage,
/// rotation) and Google JWKS validation. Auto-link by email (plan-doc §10) is
/// orchestrated by the calling handler, not this port.
/// </summary>
public interface IExternalLoginValidator
{
    Task<Result<ExternalIdentity>> ValidateAsync(string provider, string idToken, CancellationToken ct);
}

public sealed record ExternalIdentity(string ExternalId, string Email, string FirstName, string LastName);
