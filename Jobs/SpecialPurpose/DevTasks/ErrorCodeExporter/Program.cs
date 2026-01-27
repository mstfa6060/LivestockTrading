using System.Reflection;
using Common.Services.ErrorCodeGenerator;

// Ambiguity fix
using CommonDomainErrors = Common.Definitions.Domain.Errors.DomainErrors;
using LivestockTradingDomainErrors = LivestockTrading.Domain.Errors.LivestockTradingDomainErrors;

// =====================
// GlobalLivestock kokunu bul
// =====================
static string GetProjectRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir != null)
    {
        if (string.Equals(dir.Name, "backend", StringComparison.OrdinalIgnoreCase)
            && dir.Parent != null)
            return dir.Parent.FullName;

        dir = dir.Parent;
    }
    throw new Exception("GlobalLivestock koku (backend'in parent'i) bulunamadi.");
}

// ==========================================
// Cikti yollarini proje kokune gore uret
// ==========================================
static Dictionary<string, Type> GetPathsAndTypes()
{
    var projectRoot = GetProjectRoot(); // Orn: D:\Projects\GlobalLivestock
    var pathsAndTypes = new Dictionary<string, Type>();

    // 48 dil destegi
    var languages = new[]
    {
        "en","tr","es","fr","de","ar","pt","ru","hi","zh","ja","it","nl","ko",
        "sv","no","da","fi","pl","cs","el","he","hu","ro","sk","uk","vi","id",
        "ms","th","bn","ta","te","mr","fa","ur","bg","hr","sr","sl","lt","lv",
        "et","sw","af","is","ga","mt","am","hy"
    };

    foreach (var lang in languages)
    {
        // --- Common DomainErrors (IAM, Auth, User, vb.) ---
        // Web
        pathsAndTypes.Add(
            Path.Combine(projectRoot, "web", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "common", $"{lang}.ts"),
            typeof(CommonDomainErrors));

        // Mobile
        pathsAndTypes.Add(
            Path.Combine(projectRoot, "mobil", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "common", $"{lang}.ts"),
            typeof(CommonDomainErrors));

        // --- LivestockTrading DomainErrors ---
        // Web
        pathsAndTypes.Add(
            Path.Combine(projectRoot, "web", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "livestocktrading", $"{lang}.ts"),
            typeof(LivestockTradingDomainErrors));

        // Mobile
        pathsAndTypes.Add(
            Path.Combine(projectRoot, "mobil", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "livestocktrading", $"{lang}.ts"),
            typeof(LivestockTradingDomainErrors));
    }

    return pathsAndTypes;
}

// ============================
// Typescript icerik ureten kisim
// ============================
static string GetErrorAsTypescriptFiles(Dictionary<string, string> lines, Type myType)
{
    try
    {
        Type[] nestType = myType.GetNestedTypes();
        var output = "export default {\n  translation: {\n    error: {";

        foreach (Type type in nestType)
        {
            var properties = type.GetProperties().ToList();

            properties.ForEach((property) =>
            {
                if (property.PropertyType == typeof(string))
                {
                    var propertyName = ErrorCodeGenerator.GetFrontendName(property.Name);
                    var propertyValue = property.GetValue(null, null);
                    var line = lines.FirstOrDefault(l => l.Key == propertyName);
                    var escapedValue = propertyValue?.ToString()?.Replace("\\", "\\\\")?.Replace("\"", "\\\"");

                    // Degerin bos olup olmadigini kontrol et
                    if (!string.IsNullOrEmpty(line.Key) && line.Value != "\"\"" && !string.IsNullOrWhiteSpace(line.Value))
                        output += $"\n      {propertyName}: {line.Value},";
                    else
                        output += $"\n      {propertyName}: \"{escapedValue}\",";
                }
            });
        }

        output += "\n    }\n  }\n};";
        return output;
    }
    catch (Exception e)
    {
        return $"Error: {e.Message}";
    }
}

// ============================
// Var olan dosyayi oku (varsa)
// ============================
static async Task<Dictionary<string, string>> Read(string path)
{
    if (!File.Exists(path))
        return new Dictionary<string, string>();

    var content = new Dictionary<string, string>();
    using var sr = new StreamReader(path);
    var templateContent = await sr.ReadToEndAsync();

    // Parser
    var cleaned = templateContent
        .Replace("export const errors = {", "")
        .Replace("export default {", "")
        .Replace("translation:", "")
        .Replace("error:", "")
        .Replace("{", "")
        .Replace("}", "")
        .Replace(";", "")
        .Replace("\n", "")
        .Replace("\r", "");

    foreach (var item in cleaned.Split(','))
    {
        if (string.IsNullOrWhiteSpace(item) || !item.Contains(':')) continue;
        var idx = item.IndexOf(':');
        var key = item.Substring(0, idx).Trim();
        var value = item.Substring(idx + 1).Trim();
        if (!content.ContainsKey(key) && key.Length > 0)
            content.Add(key, value);
    }
    return content;
}

// ============================
// Yaz ve klasoru olustur
// ============================
static async Task<bool> Write(string path, string template)
{
    try
    {
        var directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath!);
            Console.WriteLine($"Klasor olusturuldu: {directoryPath}");
        }

        await File.WriteAllTextAsync(path, template);
        return true;
    }
    catch (Exception e)
    {
        Console.WriteLine($"Hata: {e.Message}");
        return false;
    }
}

// ============================
// MAIN
// ============================
Console.WriteLine("ErrorCodeExporter baslatildi...");
Console.WriteLine($"Proje koku: {GetProjectRoot()}");
Console.WriteLine();

var pathsAndTypes = GetPathsAndTypes();
var successCount = 0;
var failCount = 0;

foreach (var kv in pathsAndTypes)
{
    var path = kv.Key;
    var type = kv.Value;

    var existing = await Read(path);
    var output = GetErrorAsTypescriptFiles(existing, type);
    var ok = await Write(path, output);

    if (ok)
    {
        successCount++;
        Console.WriteLine($"[OK] {Path.GetFileName(path)} -> {path}");
    }
    else
    {
        failCount++;
        Console.WriteLine($"[FAIL] {path}");
    }
}

Console.WriteLine();
Console.WriteLine($"Tamamlandi: {successCount} basarili, {failCount} basarisiz");
Console.WriteLine($"Toplam: {pathsAndTypes.Count} dosya ({pathsAndTypes.Count / 4} dil x 4 hedef)");
