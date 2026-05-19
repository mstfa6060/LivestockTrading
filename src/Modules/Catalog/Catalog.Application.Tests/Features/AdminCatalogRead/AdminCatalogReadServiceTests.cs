using LivestockTrading.Catalog.Application.Features.AdminCatalogRead;
using Shared.Contracts.Catalog.Admin;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.AdminCatalogRead;

public class AdminCatalogReadServiceTests
{
    private readonly AdminCatalogReadService _service = new();

    [Fact]
    public async Task ListBrandsAsync_Throws_NotImplementedException()
    {
        await Assert.ThrowsAsync<NotImplementedException>(
            () => _service.ListBrandsAsync(new BrandListFilter(null, null), CancellationToken.None));
    }

    [Fact]
    public async Task GetMissingTranslationsAsync_Throws_NotImplementedException()
    {
        await Assert.ThrowsAsync<NotImplementedException>(
            () => _service.GetMissingTranslationsAsync("en", CancellationToken.None));
    }

    [Fact]
    public async Task GetRateLogsAsync_Throws_NotImplementedException()
    {
        await Assert.ThrowsAsync<NotImplementedException>(
            () => _service.GetRateLogsAsync(7, CancellationToken.None));
    }
}
