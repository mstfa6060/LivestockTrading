namespace Livestock.Features.ProductViewHistories;

public record ProductViewHistoryItem(
    Guid Id, Guid ProductId, string? ProductTitle,
    DateTime ViewedAt, string? ViewSource);

public record RecordProductViewRequest(Guid ProductId, string? ViewSource);
public record GetProductViewHistoryRequest(int Page = 1, int PageSize = 50);
