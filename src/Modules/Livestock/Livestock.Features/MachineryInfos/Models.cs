using Livestock.Domain.Enums;

namespace Livestock.Features.MachineryInfos;

public record MachineryInfoDetail(
    Guid Id, Guid ProductId,
    MachineryType MachineryType,
    string? Manufacturer, string? Model, int? Year,
    decimal? HorsePower, int? WorkingHours, string? FuelType,
    string? Dimensions, decimal? WeightKg,
    bool HasWarranty, DateTime? WarrantyExpiresAt,
    string? SerialNumber, string? AdditionalFeatures, DateTime CreatedAt);

public record CreateMachineryInfoRequest(
    Guid ProductId,
    MachineryType MachineryType,
    string? Manufacturer, string? Model, int? Year,
    decimal? HorsePower, int? WorkingHours, string? FuelType,
    string? Dimensions, decimal? WeightKg,
    bool HasWarranty, DateTime? WarrantyExpiresAt,
    string? SerialNumber, string? AdditionalFeatures);

public record UpdateMachineryInfoRequest(
    Guid Id,
    MachineryType MachineryType,
    string? Manufacturer, string? Model, int? Year,
    decimal? HorsePower, int? WorkingHours, string? FuelType,
    string? Dimensions, decimal? WeightKg,
    bool HasWarranty, DateTime? WarrantyExpiresAt,
    string? SerialNumber, string? AdditionalFeatures);

public record GetMachineryInfoByProductRequest(Guid ProductId);
public record GetMachineryInfoRequest(Guid Id);
public record DeleteMachineryInfoRequest(Guid Id);
