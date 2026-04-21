namespace Livestock.Features.Faqs;

public record FaqItem(
    Guid Id,
    string Question,
    string Answer,
    string? Category,
    int SortOrder,
    bool IsActive,
    string? LanguageCode,
    DateTime CreatedAt);

public record CreateFaqRequest(
    string Question,
    string Answer,
    string? Category,
    int SortOrder,
    bool IsActive,
    string? LanguageCode);

public record UpdateFaqRequest(
    Guid Id,
    string Question,
    string Answer,
    string? Category,
    int SortOrder,
    bool IsActive,
    string? LanguageCode);

public record GetFaqsRequest(string? LanguageCode = null, string? Category = null);
public record GetFaqRequest(Guid Id);
public record DeleteFaqRequest(Guid Id);
