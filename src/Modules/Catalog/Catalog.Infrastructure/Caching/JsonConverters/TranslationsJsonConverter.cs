using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Infrastructure.Caching.JsonConverters;

/// <summary>
/// F-S51 fix: Translations sealed class Pure POCO Domain saf (Translations.cs:5) —
/// parameterless ctor + setter YOK, STJ default deser uyumsuz. Hexagonal port-adapter:
/// converter Infrastructure tarafinda, Domain dokunulmaz (KAYDET-7/W1-4 hiyerarsi:
/// Kernel baskin, Infrastructure cozum uretir).
///
/// JSON sema: {"tr":"...","en":"...","ar":"..."} obje — key 2-char lowercase ISO 639-1
/// (LanguageCode VO ctor ToLowerInvariant normalize), value plain string.
/// `Translations?` (nullable) field icin JSON `null` token destekli (`Read` null doner,
/// caller-side nullable atama).
/// </summary>
internal sealed class TranslationsJsonConverter : JsonConverter<Translations>
{
    public override Translations? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Translations bekleniyor: StartObject veya Null; alindi: {reader.TokenType}.");

        var map = new Dictionary<LanguageCode, string>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return new Translations(map);

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"Translations property name bekleniyor; alindi: {reader.TokenType}.");

            var propertyName = reader.GetString()
                ?? throw new JsonException("Translations property name null olamaz.");

            if (!reader.Read() || reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Translations '{propertyName}' icin string deger bekleniyor.");

            var value = reader.GetString()
                ?? throw new JsonException($"Translations '{propertyName}' icin string deger null olamaz.");

            map[new LanguageCode(propertyName)] = value;
        }

        throw new JsonException("Translations EndObject token bulunamadi (truncated JSON).");
    }

    public override void Write(Utf8JsonWriter writer, Translations value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var entry in value.Map)
        {
            // LanguageCode VO zaten lowercase normalize (ctor ToLowerInvariant);
            // .ToString() = .Value = lowercase 2-char code.
            writer.WriteString(entry.Key.ToString(), entry.Value);
        }
        writer.WriteEndObject();
    }
}
