using LivestockTrading.Catalog.Application.Common.Mappers;
using Shared.Domain;
using Xunit;
using ContractsKind = Shared.Contracts.Catalog.BorderRuleKind;
using DomainKind = LivestockTrading.Catalog.Domain.Aggregates.BorderRuleKind;

namespace LivestockTrading.Catalog.Application.Tests.Common.Mappers;

public class BorderRuleKindMapperTests
{
    [Theory]
    [InlineData(ContractsKind.Banned, DomainKind.Banned)]
    [InlineData(ContractsKind.RequiresCert, DomainKind.RequiresCert)]
    [InlineData(ContractsKind.RequiresQuarantine, DomainKind.RequiresQuarantine)]
    [InlineData(ContractsKind.AdditionalFee, DomainKind.AdditionalFee)]
    [InlineData(ContractsKind.QuantityLimit, DomainKind.QuantityLimit)]
    public void ToDomain_MapsEachContractsKind_ToDomainKind(ContractsKind input, DomainKind expected)
        => Assert.Equal(expected, BorderRuleKindMapper.ToDomain(input));

    [Fact]
    public void ToDomain_WhenUnknownContractsKind_ThrowsDomainException()
        => Assert.Throws<DomainException>(() => BorderRuleKindMapper.ToDomain((ContractsKind)999));
}
