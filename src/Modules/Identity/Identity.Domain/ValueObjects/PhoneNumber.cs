namespace LivestockTrading.Identity.Domain.ValueObjects;
using PhoneNumbers;
using Shared.Domain;

/// <summary>E.164 normalized + libphonenumber validation.</summary>
public readonly record struct PhoneNumber
{
    private readonly string? _e164;
    public string E164 => _e164 ?? throw new DomainException("PhoneNumber default instance.");

    public PhoneNumber(string raw, string defaultRegion = "TR")
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("Telefon bos olamaz.");
        var util = PhoneNumberUtil.GetInstance();
        try
        {
            var parsed = util.Parse(raw.Trim(), defaultRegion);
            if (!util.IsValidNumber(parsed))
                throw new DomainException("Telefon gecersiz.");
            _e164 = util.Format(parsed, PhoneNumberFormat.E164);
        }
        catch (NumberParseException ex)
        {
            throw new DomainException($"Telefon parse hatasi: {ex.Message}");
        }
    }

    public int CountryCallingCode
    {
        get
        {
            var util = PhoneNumberUtil.GetInstance();
            return util.Parse(E164, null).CountryCode;
        }
    }

    public string NationalNumber
    {
        get
        {
            var util = PhoneNumberUtil.GetInstance();
            return util.Parse(E164, null).NationalNumber.ToString();
        }
    }

    public override string ToString() => E164;
}
