using Livestock.Domain.Enums;

namespace Livestock.Features.Transport;

public record TransportRequestListItem(Guid Id, Guid RequesterUserId, string PickupCountryCode, string PickupCity, string DeliveryCountryCode, string DeliveryCity, TransportType TransportType, TransportRequestStatus Status, decimal? Budget, string? CurrencyCode, DateTime? PickupDate, DateTime CreatedAt);
public record TransportRequestDetail(Guid Id, Guid RequesterUserId, Guid? SellerId, Guid? ProductId, Guid? AssignedTransporterId, string PickupCountryCode, string PickupCity, string? PickupAddress, string DeliveryCountryCode, string DeliveryCity, string? DeliveryAddress, TransportType TransportType, TransportRequestStatus Status, string? CargoDescription, int? AnimalCount, decimal? EstimatedWeightKg, DateTime? PickupDate, string? SpecialRequirements, decimal? Budget, string? CurrencyCode, DateTime CreatedAt);
public record CreateTransportRequestRequest(string PickupCountryCode, string PickupCity, string? PickupAddress, double? PickupLatitude, double? PickupLongitude, string DeliveryCountryCode, string DeliveryCity, string? DeliveryAddress, double? DeliveryLatitude, double? DeliveryLongitude, TransportType TransportType, string? CargoDescription, int? AnimalCount, decimal? EstimatedWeightKg, DateTime? PickupDate, string? SpecialRequirements, decimal? Budget, string? CurrencyCode, Guid? ProductId);
public record GetTransportRequestRequest(Guid Id);

public record TransportOfferListItem(Guid Id, Guid TransportRequestId, Guid TransporterId, string TransporterName, decimal Price, string CurrencyCode, int? EstimatedDaysMin, int? EstimatedDaysMax, TransportOfferStatus Status, DateTime CreatedAt);
public record CreateTransportOfferRequest(Guid TransportRequestId, decimal Price, string CurrencyCode, string? Note, int? EstimatedDaysMin, int? EstimatedDaysMax);
public record AcceptTransportOfferRequest(Guid Id);
public record RejectTransportOfferRequest(Guid Id);
public record GetTransportOffersByRequestRequest(Guid TransportRequestId);

public record TrackingUpdateItem(Guid Id, TrackingStatus Status, string? Note, string? Location, DateTime OccurredAt);
public record AddTrackingUpdateRequest(Guid TransportRequestId, TrackingStatus Status, string? Note, string? Location, double? Latitude, double? Longitude);
