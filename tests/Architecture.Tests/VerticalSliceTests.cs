using System.Reflection;
using System.Runtime.CompilerServices;
using FastEndpoints;
using FluentAssertions;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Use-case-per-folder vertical slice pattern guard tests.
///
/// Each refactored slice MUST follow:
/// <code>
/// Livestock.Features/{Entity}/{UseCase}/Endpoint.cs
/// Livestock.Features/{Entity}/{UseCase}/Models.cs
/// Livestock.Features/{Entity}/{UseCase}/Validator.cs   (optional)
/// </code>
///
/// New slices added to <see cref="RefactoredNamespaces"/> when their MST refactor lands.
///
/// Plan reference: _doc/Implementation/StackMigrationPlan.md §4.8
/// </summary>
public class VerticalSliceTests
{
    /// <summary>
    /// Slice root namespaces that have been migrated to use-case-per-folder.
    /// Add the next entry only AFTER the corresponding refactor PR merges.
    /// </summary>
    private static readonly string[] RefactoredNamespaces =
    [
        "Livestock.Features.Products",
        "Livestock.Features.Offers",
        "Livestock.Features.Categories",
        // "Livestock.Features.Sellers",  // MST-76 — add when refactor merges
    ];

    /// <summary>
    /// Disk-level whitelist mirroring <see cref="RefactoredNamespaces"/>.
    /// Names are folder names under <c>Livestock.Features/</c>.
    /// </summary>
    private static readonly string[] RefactoredFolders =
    [
        "Products",
        "Offers",
        "Categories",
        // "Sellers",  // MST-76 — add when refactor merges
    ];

    private static readonly Assembly LivestockFeaturesAssembly =
        typeof(Livestock.Features.LivestockModuleRegistration).Assembly;

    [Fact]
    public void Refactored_slices_have_at_most_one_endpoint_class_per_usecase_folder()
    {
        var failures = new List<string>();

        foreach (var rootNs in RefactoredNamespaces)
        {
            var endpointTypes = LivestockFeaturesAssembly.GetTypes()
                .Where(t => t.Namespace != null
                            && (t.Namespace == rootNs || t.Namespace.StartsWith(rootNs + ".", StringComparison.Ordinal)))
                .Where(t => !t.IsAbstract
                            && !t.IsInterface
                            && typeof(IEndpoint).IsAssignableFrom(t))
                .ToList();

            // Group by full namespace — each leaf namespace is a use-case folder.
            // Use-case-per-folder means: at most ONE endpoint class per leaf namespace.
            var grouped = endpointTypes
                .GroupBy(t => t.Namespace!)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var violation in grouped)
            {
                failures.Add(
                    $"{violation.Key} contains {violation.Count()} endpoint classes: " +
                    string.Join(", ", violation.Select(t => t.Name)));
            }
        }

        failures.Should().BeEmpty(
            because: "Refactored slices must have exactly one endpoint class per use-case folder. Violations:\n"
                     + string.Join("\n", failures));
    }

    [Fact]
    public void Refactored_slices_do_not_have_legacy_fat_files()
    {
        var featuresRoot = LocateLivestockFeaturesRoot();

        var failures = new List<string>();

        foreach (var folder in RefactoredFolders)
        {
            var slicePath = Path.Combine(featuresRoot, folder);
            if (!Directory.Exists(slicePath))
            {
                continue;
            }

            // The legacy "fat" file pattern: a single Endpoints.cs / Models.cs /
            // Validators.cs at the slice root. After refactor each endpoint
            // owns its own folder + Endpoint.cs/Models.cs/Validator.cs files.
            foreach (var legacy in new[] { "Endpoints.cs", "Models.cs", "Validators.cs" })
            {
                var legacyFile = Path.Combine(slicePath, legacy);
                if (File.Exists(legacyFile))
                {
                    failures.Add(
                        $"{folder}/{legacy} should be removed — refactored slices use use-case-per-folder " +
                        "(see _doc/Implementation/StackMigrationPlan.md §4.8).");
                }
            }
        }

        failures.Should().BeEmpty(
            because: "Refactored slices must not retain the legacy fat-file layout. Violations:\n"
                     + string.Join("\n", failures));
    }

    private static string LocateLivestockFeaturesRoot([CallerFilePath] string callerPath = "")
    {
        // CallerFilePath resolves to the test source file location at compile
        // time. From tests/Architecture.Tests/VerticalSliceTests.cs we walk
        // two levels up to repo root, then dive into the Features project.
        var dir = new FileInfo(callerPath).Directory!.FullName;
        var path = Path.GetFullPath(Path.Combine(
            dir, "..", "..", "src", "Modules", "Livestock", "Livestock.Features"));

        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException(
                $"Livestock.Features klasörü bulunamadı: {path}");
        }

        return path;
    }
}
