namespace Shared.Contracts.Catalog.Admin;

/// <summary>Bulk import özeti (BulkImportBrandsAsync). BulkImportError = context-spesifik companion (aynı dosya, yüksek kohezyon).</summary>
public sealed record BulkImportResult(
    int Total,
    int SuccessCount,
    int FailureCount,
    IReadOnlyList<BulkImportError> Errors);

/// <summary>Per-row hata. RowNumber + Code discriminator: per-row validation vs sistem hatası ayrımı.</summary>
public sealed record BulkImportError(
    int RowNumber,
    string Code,
    string Message);
