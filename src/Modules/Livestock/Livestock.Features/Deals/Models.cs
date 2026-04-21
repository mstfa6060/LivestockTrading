using Livestock.Domain.Enums;

namespace Livestock.Features.Deals;

public record DealListItem(Guid Id, Guid OfferId, Guid ProductId, string ProductTitle, Guid BuyerUserId, Guid SellerId, decimal AgreePrice, string CurrencyCode, int Quantity, DealStatus Status, DeliveryMethod DeliveryMethod, DateTime CreatedAt);
public record DealDetail(Guid Id, Guid OfferId, Guid ProductId, string ProductTitle, Guid BuyerUserId, Guid SellerId, decimal AgreePrice, string CurrencyCode, int Quantity, DealStatus Status, DeliveryMethod DeliveryMethod, string? Notes, DateTime? CompletedAt, DateTime CreatedAt);
public record GetDealRequest(Guid Id);
public record UpdateDealStatusRequest(Guid Id, DealStatus Status, DeliveryMethod? DeliveryMethod, string? Notes);
public record CancelDealRequest(Guid Id, string? Reason);
public record GetAllDealsRequest(int Page = 1, int PageSize = 20);
public record DealPickItem(Guid Id, string ProductTitle, DealStatus Status, DateTime CreatedAt);
public record DealPickRequest(List<Guid>? SelectedIds, int Limit = 10);
