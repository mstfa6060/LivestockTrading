using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Modüler monolit bağımlılık kuralları.
/// Bu testler, modüller arası izin verilmeyen referansları önler.
/// </summary>
public class ModuleDependencyTests
{
    // Assembly referansları — modül eklendikçe burada tanımlanır
    private static readonly Assembly SharedAbstractions = typeof(Shared.Abstractions.Results.Error).Assembly;
    private static readonly Assembly IamDomain = typeof(Iam.Domain.IamDomainMarker).Assembly;
    private static readonly Assembly FilesDomain = typeof(Files.Domain.FilesDomainMarker).Assembly;
    private static readonly Assembly LivestockDomain = typeof(Livestock.Domain.LivestockDomainMarker).Assembly;

    [Fact]
    public void Domain_projects_should_not_reference_infrastructure()
    {
        var result = Types.InAssemblies([IamDomain, FilesDomain, LivestockDomain])
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "FastEndpoints",
                "NATS",
                "ZiggyCreatures.Caching")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "Domain projeleri altyapı bağımlılığı içermemeli");
    }

    [Fact]
    public void Iam_domain_should_not_reference_other_module_domains()
    {
        var result = Types.InAssembly(IamDomain)
            .ShouldNot()
            .HaveDependencyOnAny("Files.Domain", "Livestock.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "Iam.Domain başka modüllerin Domain'ine referans veremez");
    }

    [Fact]
    public void Files_domain_should_not_reference_other_module_domains()
    {
        var result = Types.InAssembly(FilesDomain)
            .ShouldNot()
            .HaveDependencyOnAny("Iam.Domain", "Livestock.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "Files.Domain başka modüllerin Domain'ine referans veremez");
    }

    [Fact]
    public void Livestock_domain_should_not_reference_other_module_domains()
    {
        var result = Types.InAssembly(LivestockDomain)
            .ShouldNot()
            .HaveDependencyOnAny("Iam.Domain", "Files.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "Livestock.Domain başka modüllerin Domain'ine referans veremez");
    }

    [Fact]
    public void Shared_abstractions_should_have_no_external_dependencies()
    {
        var result = Types.InAssembly(SharedAbstractions)
            .ShouldNot()
            .HaveDependencyOnAny(
                "FastEndpoints",
                "Microsoft.EntityFrameworkCore",
                "NATS",
                "Serilog",
                "ZiggyCreatures.Caching")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "Shared.Abstractions sadece .NET runtime'a bağımlı olmalı");
    }
}
