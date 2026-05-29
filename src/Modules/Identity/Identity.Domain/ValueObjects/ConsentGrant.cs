namespace LivestockTrading.Identity.Domain.ValueObjects;
using Shared.Contracts.Identity;
using Shared.Domain;

/// <summary>Consent grant (User factory parametresi). Domain internal.</summary>
public sealed record ConsentGrant
{
    public ConsentType Type { get; }
    public string Version { get; }
    public bool Granted { get; }

    public ConsentGrant(ConsentType type, string version, bool granted)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new DomainException("Consent version bos olamaz.");
        if (version.Trim().Length > 32)
            throw new DomainException("Consent version 32 karakteri asamaz.");
        Type = type;
        Version = version.Trim();
        Granted = granted;
    }
}
