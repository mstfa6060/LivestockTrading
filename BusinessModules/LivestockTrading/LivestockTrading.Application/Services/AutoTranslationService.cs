using System.Net;
using System.Text.Json;

namespace LivestockTrading.Application.Services;

/// <summary>
/// Google Translate ücretsiz endpoint kullanarak otomatik çeviri yapar.
/// Kategori oluşturma/güncelleme sırasında Name ve Description alanlarını 50+ dile çevirir.
/// </summary>
public class AutoTranslationService
{
	private static readonly HttpClient _httpClient = new()
	{
		Timeout = TimeSpan.FromSeconds(10)
	};

	// Seed data'daki tüm dil kodları
	private static readonly string[] TargetLanguages =
	{
		"en", "tr", "ar", "de", "fr", "es", "pt", "ru", "zh", "hi",
		"ja", "ko", "it", "nl", "pl", "sv", "da", "fi", "no", "cs",
		"hu", "ro", "bg", "hr", "sr", "uk", "el", "th", "vi", "id",
		"ms", "bn", "ur", "fa", "he", "az", "uz", "kk", "ka", "sw",
		"am", "ha", "ne", "mn", "sk", "sl", "et", "lv", "lt", "sq",
		"pt-BR", "zh-CN", "zh-TW"
	};

	/// <summary>
	/// Verilen metni tüm desteklenen dillere çevirir ve JSON string olarak döner.
	/// Kaynak dil otomatik algılanır.
	/// </summary>
	public async Task<string> TranslateToAllLanguages(string text, string sourceLang = "auto")
	{
		if (string.IsNullOrWhiteSpace(text))
			return null;

		var translations = new Dictionary<string, string>();

		foreach (var lang in TargetLanguages)
		{
			try
			{
				// pt-BR → pt, zh-CN → zh-cn (Google format)
				var googleLang = lang.ToLower() switch
				{
					"pt-br" => "pt",
					"zh-cn" => "zh-CN",
					"zh-tw" => "zh-TW",
					_ => lang.ToLower()
				};

				var translated = await TranslateSingle(text, sourceLang, googleLang);

				if (!string.IsNullOrWhiteSpace(translated))
					translations[lang] = translated;
				else
					translations[lang] = text; // Fallback: orijinal metin
			}
			catch
			{
				translations[lang] = text; // Hata durumunda orijinal metin
			}

			// Rate limiting - Google'ın ücretsiz endpoint'i için gerekli
			await Task.Delay(50);
		}

		return JsonSerializer.Serialize(translations);
	}

	/// <summary>
	/// Mevcut çevirileri koruyarak eksik dilleri çevirir.
	/// Frontend'den gelen kısmi çeviriler varsa onları korur.
	/// </summary>
	public async Task<string> FillMissingTranslations(string existingTranslationsJson, string fallbackText, string sourceLang = "auto")
	{
		var existing = new Dictionary<string, string>();

		if (!string.IsNullOrWhiteSpace(existingTranslationsJson))
		{
			try
			{
				existing = JsonSerializer.Deserialize<Dictionary<string, string>>(existingTranslationsJson)
					?? new Dictionary<string, string>();
			}
			catch (JsonException)
			{
				existing = new Dictionary<string, string>();
			}
		}

		// Tüm diller zaten doluysa çeviri yapma
		var missingLangs = TargetLanguages.Where(l => !existing.ContainsKey(l) || string.IsNullOrWhiteSpace(existing[l])).ToList();

		if (missingLangs.Count == 0)
			return existingTranslationsJson;

		var textToTranslate = fallbackText;

		foreach (var lang in missingLangs)
		{
			try
			{
				var googleLang = lang.ToLower() switch
				{
					"pt-br" => "pt",
					"zh-cn" => "zh-CN",
					"zh-tw" => "zh-TW",
					_ => lang.ToLower()
				};

				var translated = await TranslateSingle(textToTranslate, sourceLang, googleLang);

				if (!string.IsNullOrWhiteSpace(translated))
					existing[lang] = translated;
				else
					existing[lang] = textToTranslate;
			}
			catch
			{
				existing[lang] = textToTranslate;
			}

			await Task.Delay(50);
		}

		return JsonSerializer.Serialize(existing);
	}

	private static async Task<string> TranslateSingle(string text, string sourceLang, string targetLang)
	{
		var encodedText = WebUtility.UrlEncode(text);
		var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={encodedText}";

		var response = await _httpClient.GetAsync(url);
		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadAsStringAsync();

		// Response format: [[["translated text","source text",null,null,10]],null,"en",...]
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;

		// Birden fazla cümle olabilir, hepsini birleştir
		var result = "";
		var sentences = root[0];
		for (int i = 0; i < sentences.GetArrayLength(); i++)
		{
			result += sentences[i][0].GetString();
		}

		return result;
	}
}
