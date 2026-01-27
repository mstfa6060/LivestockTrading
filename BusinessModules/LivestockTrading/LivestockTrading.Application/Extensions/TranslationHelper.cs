using System.Text.Json;

namespace LivestockTrading.Application.Extensions;

/// <summary>
/// JSON formatındaki çeviri alanlarından istenen dildeki metni çıkarır
/// </summary>
public static class TranslationHelper
{
	private static readonly JsonSerializerOptions _jsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	/// <summary>
	/// JSON çeviri alanından istenen dildeki metni döndürür
	/// </summary>
	/// <param name="translationsJson">JSON formatında çeviriler: {"en":"Livestock","tr":"Hayvancılık"}</param>
	/// <param name="languageCode">İstenen dil kodu (örn: "tr", "en", "de")</param>
	/// <param name="fallbackValue">Çeviri bulunamazsa kullanılacak varsayılan değer</param>
	/// <returns>Çevrilmiş metin veya fallback değer</returns>
	public static string GetTranslation(string translationsJson, string languageCode, string fallbackValue = null)
	{
		if (string.IsNullOrWhiteSpace(translationsJson))
			return fallbackValue;

		if (string.IsNullOrWhiteSpace(languageCode))
			return fallbackValue;

		try
		{
			var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(translationsJson, _jsonOptions);

			if (translations == null)
				return fallbackValue;

			// Tam eşleşme (örn: "tr")
			if (translations.TryGetValue(languageCode.ToLowerInvariant(), out var translation))
				return translation;

			// Büyük/küçük harf duyarsız arama
			var key = translations.Keys.FirstOrDefault(k =>
				k.Equals(languageCode, StringComparison.OrdinalIgnoreCase));

			if (key != null)
				return translations[key];

			// İngilizce fallback
			if (translations.TryGetValue("en", out var englishTranslation))
				return englishTranslation;

			// İlk mevcut çeviriyi döndür
			return translations.Values.FirstOrDefault() ?? fallbackValue;
		}
		catch (JsonException)
		{
			return fallbackValue;
		}
	}
}
