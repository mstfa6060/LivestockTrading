namespace Shared.ValueObjects;

/// <summary>
/// Çok-dilli metin VO. Internal Map LanguageCode-keyed (3e); API string-locale (05-catalog §9).
/// Pure POCO — STJ attribute/converter YOK; JSONB map'i C3 Infra EF value converter (Domain saf).
/// İçerik-eşitlikli (sıra-bağımsız).
/// </summary>
public sealed class Translations
{
    private readonly Dictionary<LanguageCode, string> _map;

    public static Translations Empty { get; } = new(new Dictionary<LanguageCode, string>());

    public Translations(IReadOnlyDictionary<LanguageCode, string> map)
        => _map = new Dictionary<LanguageCode, string>(map);

    public bool IsEmpty => _map.Count == 0;

    public bool TryGet(string locale, out string? value)
    {
        value = null;
        if (string.IsNullOrWhiteSpace(locale) || locale.Length != 2
            || !char.IsAsciiLetter(locale[0]) || !char.IsAsciiLetter(locale[1]))
            return false;
        return _map.TryGetValue(new LanguageCode(locale), out value);
    }

    public bool TryGetIgnoreCase(string locale, out string? value)
        => TryGet(locale, out value); // LanguageCode ctor zaten lowercase normalize ediyor

    public string FirstOrEmpty() => _map.Count == 0 ? string.Empty : _map.Values.First();

    public IReadOnlyDictionary<LanguageCode, string> Map => _map;

    public override bool Equals(object? obj)
        => obj is Translations o && _map.Count == o._map.Count
           && _map.All(kv => o._map.TryGetValue(kv.Key, out var v) && v == kv.Value);

    public override int GetHashCode()
    {
        var h = new HashCode();
        foreach (var kv in _map.OrderBy(k => k.Key.Value))
        {
            h.Add(kv.Key);
            h.Add(kv.Value);
        }
        return h.ToHashCode();
    }
}
