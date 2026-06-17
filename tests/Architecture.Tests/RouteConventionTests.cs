using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Architecture.Tests;

public class RouteConventionTests
{
    private static readonly Regex HttpMethodCall = new(
        @"\b(Get|Post|Put|Delete|Patch)\(""(/[^""]*)""\)",
        RegexOptions.Compiled);

    private static readonly string[] AllowedPrefixes =
    [
        "/iam/",
        "/fileprovider/",
        "/livestocktrading/",
    ];

    [Fact]
    public void All_endpoints_must_use_POST_with_module_prefix()
    {
        var modulesRoot = LocateModulesRoot();
        var endpointFiles = Directory.EnumerateFiles(modulesRoot, "Endpoint*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                     && !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .ToList();

        endpointFiles.Should().NotBeEmpty(
            because: "src/Modules altında en az bir Endpoint*.cs dosyası bulunmalı");

        var violations = new List<string>();

        foreach (var file in endpointFiles)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var match = HttpMethodCall.Match(lines[i]);
                if (!match.Success)
                {
                    continue;
                }

                var verb = match.Groups[1].Value;
                var route = match.Groups[2].Value;

                if (verb != "Post")
                {
                    violations.Add($"{file}:{i + 1} — '{verb}(\"{route}\")' kullanıyor; legacy uyumluluk için tüm endpoint'ler POST olmalı");
                    continue;
                }

                if (!AllowedPrefixes.Any(p => route.StartsWith(p, StringComparison.Ordinal)))
                {
                    violations.Add($"{file}:{i + 1} — Route '{route}' bir module prefix ile başlamalı: /iam/, /fileprovider/ veya /livestocktrading/");
                }
            }
        }

        violations.Should().BeEmpty(
            because: "Frontend tüm endpoint'leri POST /{module}/{Entity}/{Action} kalıbında bekliyor. İhlaller:\n" + string.Join("\n", violations));
    }

    private static string LocateModulesRoot([CallerFilePath] string callerPath = "")
    {
        var dir = new FileInfo(callerPath).Directory!.FullName;
        var modulesRoot = Path.GetFullPath(Path.Combine(dir, "..", "..", "src", "Modules"));
        if (!Directory.Exists(modulesRoot))
        {
            throw new DirectoryNotFoundException($"src/Modules klasörü bulunamadı: {modulesRoot}");
        }
        return modulesRoot;
    }
}
