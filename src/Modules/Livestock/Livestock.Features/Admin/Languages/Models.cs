namespace Livestock.Features.Admin.Languages;

public record LanguageItem(
    Guid Id,
    string Code,
    string Name,
    string? NativeName,
    bool IsRightToLeft,
    bool IsActive,
    bool IsDefault,
    int SortOrder,
    string? FlagIconUrl,
    DateTime CreatedAt);

public record GetLanguageRequest(Guid Id);

public record CreateLanguageRequest(
    string Code,
    string Name,
    string? NativeName,
    bool IsRightToLeft = false,
    bool IsActive = true,
    bool IsDefault = false,
    int SortOrder = 0,
    string? FlagIconUrl = null);

public record UpdateLanguageRequest(
    Guid Id,
    string Name,
    string? NativeName,
    bool IsRightToLeft,
    bool IsActive,
    bool IsDefault,
    int SortOrder,
    string? FlagIconUrl);

public record DeleteLanguageRequest(Guid Id);
