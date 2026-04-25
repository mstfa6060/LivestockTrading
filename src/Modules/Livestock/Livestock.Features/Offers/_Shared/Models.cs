using Livestock.Domain.Enums;

namespace Livestock.Features.Offers;

public record OfferListItem(Guid Id, Guid ProductId, string ProductTitle, Guid BuyerUserId, Guid SellerId, decimal OfferedPrice, string CurrencyCode, int Quantity, OfferStatus Status, DateTime CreatedAt);
public record OfferDetail(Guid Id, Guid ProductId, string ProductTitle, Guid BuyerUserId, Guid SellerId, decimal OfferedPrice, string CurrencyCode, int Quantity, string? Note, OfferStatus Status, decimal? CounterPrice, string? CounterNote, DateTime? ExpiresAt, DateTime CreatedAt);
