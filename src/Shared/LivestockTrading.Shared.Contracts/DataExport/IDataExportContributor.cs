namespace Shared.Contracts.DataExport;

/// <summary>GDPR data export per-modül katkısı (05-identity §11). Catalog hariç tüm modüller implement eder.</summary>
public interface IDataExportContributor
{
    string ModuleName { get; }
    Task<object> ExportAsync(Guid userId, CancellationToken ct);
}
