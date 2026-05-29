namespace LivestockTrading.Identity.Domain.ValueObjects;
using System.Net.Mail;
using Shared.Domain;

/// <summary>RFC 5322 lite (BCL MailAddress.TryCreate) + normalized lowercase. MailKit YOK.</summary>
public readonly record struct EmailAddress
{
    private readonly string? _value;
    public string Value => _value ?? throw new DomainException("EmailAddress default instance.");

    public EmailAddress(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("Email bos olamaz.");
        var trimmed = raw.Trim();
        if (trimmed.Length > 254)
            throw new DomainException("Email 254 karakteri asamaz (RFC 5321).");
        if (!MailAddress.TryCreate(trimmed, out _))
            throw new DomainException("Email format gecersiz.");
        _value = trimmed.ToLowerInvariant();
    }

    public override string ToString() => Value;
}
