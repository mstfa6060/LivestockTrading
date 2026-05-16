namespace Shared.Pagination;

/// <summary>Cursor sayfası. TotalCount: admin'de dolu, hot path null (count query pahalı).</summary>
public sealed record CursorPage<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasMore,
    int? TotalCount = null);
