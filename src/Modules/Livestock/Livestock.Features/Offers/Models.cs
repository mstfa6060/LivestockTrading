using Livestock.Domain.Enums;

namespace Livestock.Features.Offers;

public record OfferListItem(Guid Id, Guid ProductId, string ProductTitle, Guid BuyerUserId, Guid SellerId, decimal OfferedPrice, string CurrencyCode, int Quantity, OfferStatus Status, DateTime CreatedAt);
public record OfferDetail(Guid Id, Guid ProductId, string ProductTitle, Guid BuyerUserId, Guid SellerId, decimal OfferedPrice, string CurrencyCode, int Quantity, string? Note, OfferStatus Status, decimal? CounterPrice, string? CounterNote, DateTime? ExpiresAt, DateTime CreatedAt);
public record CreateOfferRequest(Guid ProductId, decimal OfferedPrice, string CurrencyCode, int Quantity, string? Note);
public record CounterOfferRequest(Guid Id, decimal CounterPrice, string? CounterNote);
public record AcceptOfferRequest(Guid Id);
public record RejectOfferRequest(Guid Id);
public record GetOfferRequest(Guid Id);
public record WithdrawOfferRequest(Guid Id);
public record GetAllOffersRequest(int Page = 1, int PageSize = 20);
