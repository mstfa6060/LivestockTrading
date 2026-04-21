using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace LivestockTrading.Application.Services;

/// <summary>
/// Google Translate ücretsiz endpoint kullanarak otomatik çeviri yapar.
/// Kategori oluşturma/güncelleme sırasında Name ve Description alanlarını 50+ dile çevirir.
/// Paralel çeviri ile ~2-3 saniyede tamamlar.
/// </summary>
public class AutoTranslationService
{
	private static readonly HttpClient _httpClient = new()
	{
		Timeout = TimeSpan.FromSeconds(10)
	};

	// Aynı anda max paralel istek (Google rate limit'e takılmamak için)
	private const int MaxParallelism = 5;

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
	/// Verilen metni tüm desteklenen dillere paralel çevirir ve JSON string olarak döner.
	/// </summary>
	public async Task<string> TranslateToAllLanguages(string text, string sourceLang = "tr")
	{
		if (string.IsNullOrWhiteSpace(text))
			return null;

		var translations = await TranslateParallel(text, sourceLang, TargetLanguages);
		return JsonSerializer.Serialize(translations);
	}

	/// <summary>
	/// Mevcut çevirileri koruyarak eksik dilleri paralel çevirir.
	/// </summary>
	public async Task<string> FillMissingTranslations(string existingTranslationsJson, string fallbackText, string sourceLang = "tr")
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

		var missingLangs = TargetLanguages
			.Where(l => !existing.ContainsKey(l) || string.IsNullOrWhiteSpace(existing[l]))
			.ToArray();

		if (missingLangs.Length == 0)
			return existingTranslationsJson;

		var newTranslations = await TranslateParallel(fallbackText, sourceLang, missingLangs);

		foreach (var kv in newTranslations)
			existing[kv.Key] = kv.Value;

		return JsonSerializer.Serialize(existing);
	}

	private async Task<Dictionary<string, string>> TranslateParallel(string text, string sourceLang, string[] langs)
	{
		var results = new ConcurrentDictionary<string, string>();
		var semaphore = new SemaphoreSlim(MaxParallelism);

		var tasks = langs.Select(async lang =>
		{
			await semaphore.WaitAsync();
			try
			{
				var googleLang = lang.ToLower() switch
				{
					"pt-br" => "pt",
					"zh-cn" => "zh-CN",
					"zh-tw" => "zh-TW",
					_ => lang.ToLower()
				};

				var translated = await TranslateSingle(text, sourceLang, googleLang);
				results[lang] = !string.IsNullOrWhiteSpace(translated) ? translated : text;
			}
			catch
			{
				results[lang] = text;
			}
			finally
			{
				semaphore.Release();
			}
		});

		await Task.WhenAll(tasks);
		return new Dictionary<string, string>(results);
	}

	private static async Task<string> TranslateSingle(string text, string sourceLang, string targetLang)
	{
		var encodedText = WebUtility.UrlEncode(text);
		var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={encodedText}";

		// Retry with backoff (rate limit koruması)
		for (int attempt = 0; attempt < 3; attempt++)
		{
			try
			{
				var response = await _httpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();

				var json = await response.Content.ReadAsStringAsync();

				using var doc = JsonDocument.Parse(json);
				var root = doc.RootElement;

				var result = "";
				var sentences = root[0];
				for (int i = 0; i < sentences.GetArrayLength(); i++)
				{
					result += sentences[i][0].GetString();
				}

				return result;
			}
			catch when (attempt < 2)
			{
				await Task.Delay(200 * (attempt + 1));
			}
		}

		return null;
	}
}
