using Livestock.Domain.Enums;

namespace Livestock.Features.Admin.TaxRates;

public record TaxRateItem(
    Guid Id,
    string CountryCode,
    string? StateCode,
    string TaxName,
    double Rate,
    TaxType Type,
    Guid? CategoryId,
    bool IsActive,
    DateTime? ValidFrom,
    DateTime? ValidUntil,
    DateTime CreatedAt);

public record GetTaxRateRequest(Guid Id);
public record GetTaxRatesByCountryRequest(string CountryCode);

public record CreateTaxRateRequest(
    string CountryCode,
    string? StateCode,
    string TaxName,
    double Rate,
    TaxType Type = TaxType.VAT,
    Guid? CategoryId = null,
    bool IsActive = true,
    DateTime? ValidFrom = null,
    DateTime? ValidUntil = null);

public record UpdateTaxRateRequest(
    Guid Id,
    string CountryCode,
    string? StateCode,
    string TaxName,
    double Rate,
    TaxType Type,
    Guid? CategoryId,
    bool IsActive,
    DateTime? ValidFrom,
    DateTime? ValidUntil);

public record DeleteTaxRateRequest(Guid Id);
