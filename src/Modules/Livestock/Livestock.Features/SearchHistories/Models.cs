namespace Livestock.Features.SearchHistories;

public record SearchHistoryItem(
    Guid Id, string SearchQuery, string? Filters,
    int ResultsCount, DateTime SearchedAt);

public record RecordSearchRequest(string SearchQuery, string? Filters, int ResultsCount);
public record GetSearchHistoryRequest(int Page = 1, int PageSize = 50);
