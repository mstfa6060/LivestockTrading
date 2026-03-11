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
// SADECE tr.ts dosyalari uretilir.
// Diger diller translate-errors.js ile cevirilir.
// ==========================================
static Dictionary<string, Type> GetPathsAndTypes()
{
    var projectRoot = GetProjectRoot(); // Orn: D:\Projects\GlobalLivestock
    var pathsAndTypes = new Dictionary<string, Type>();

    // Sadece tr.ts (kaynak dil) uretilir
    // Diger dillerin cevirisi: web/scripts/translate-errors.js

    // --- Common DomainErrors (IAM, Auth, User, vb.) ---
    // Web
    pathsAndTypes.Add(
        Path.Combine(projectRoot, "web", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "common", "tr.ts"),
        typeof(CommonDomainErrors));

    // Mobile
    pathsAndTypes.Add(
        Path.Combine(projectRoot, "mobil", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "common", "tr.ts"),
        typeof(CommonDomainErrors));

    // --- LivestockTrading DomainErrors ---
    // Web
    pathsAndTypes.Add(
        Path.Combine(projectRoot, "web", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "livestocktrading", "tr.ts"),
        typeof(LivestockTradingDomainErrors));

    // Mobile
    pathsAndTypes.Add(
        Path.Combine(projectRoot, "mobil", "common", "livestock-api", "src", "errors", "locales", "modules", "backend", "livestocktrading", "tr.ts"),
        typeof(LivestockTradingDomainErrors));

    return pathsAndTypes;
}

// ============================
// Typescript icerik ureten kisim (sadece tr.ts icin)
// C# property degerlerini dogrudan yazar - kaynak dil (Turkce)
// ============================
static string GenerateTurkishErrorFile(Type myType)
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
                    var escapedValue = propertyValue?.ToString()?.Replace("\\", "\\\\")?.Replace("\"", "\\\"");
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
Console.WriteLine("NOT: Sadece tr.ts (kaynak dil) uretilir.");
Console.WriteLine("     Diger diller icin: cd web && node scripts/translate-errors.js --missing");
Console.WriteLine();

var pathsAndTypes = GetPathsAndTypes();
var successCount = 0;
var failCount = 0;

foreach (var kv in pathsAndTypes)
{
    var path = kv.Key;
    var type = kv.Value;

    var output = GenerateTurkishErrorFile(type);
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
Console.WriteLine($"Toplam: {pathsAndTypes.Count} dosya (tr.ts x {pathsAndTypes.Count} hedef)");
