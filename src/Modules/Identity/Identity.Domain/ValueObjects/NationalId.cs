namespace LivestockTrading.Identity.Domain.ValueObjects;
using Shared.Domain;

/// <summary>TC Kimlik Numarasi - 11 digit + NVI checksum.</summary>
public readonly record struct NationalId
{
    private readonly string? _value;
    public string Value => _value ?? throw new DomainException("NationalId default instance.");

    public NationalId(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("NationalId bos olamaz.");
        var trimmed = raw.Trim();
        if (trimmed.Length != 11 || !trimmed.All(char.IsDigit))
            throw new DomainException("NationalId 11 haneli rakam olmali.");
        if (trimmed[0] == '0')
            throw new DomainException("NationalId ilk hane 0 olamaz.");

        var digits = trimmed.Select(c => c - '0').ToArray();
        var oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        var evenSum = digits[1] + digits[3] + digits[5] + digits[7];
        var d10 = ((oddSum * 7) - evenSum) % 10;
        if (d10 < 0) d10 += 10;
        if (d10 != digits[9])
            throw new DomainException("NationalId checksum (10. hane) gecersiz.");

        var totalSum = digits.Take(10).Sum();
        if (totalSum % 10 != digits[10])
            throw new DomainException("NationalId checksum (11. hane) gecersiz.");

        _value = trimmed;
    }

    public override string ToString() => Value;
}
