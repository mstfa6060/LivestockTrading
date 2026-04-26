namespace Livestock.Features.HealthRecords;

public record HealthRecordDetail(
    Guid Id, Guid AnimalInfoId,
    string Diagnosis, string? Treatment,
    string? VetName, string? VetClinic,
    DateTime TreatmentDate, string? Notes, DateTime CreatedAt);

public record CreateHealthRecordRequest(
    Guid AnimalInfoId,
    string Diagnosis, string? Treatment,
    string? VetName, string? VetClinic,
    DateTime TreatmentDate, string? Notes);

public record UpdateHealthRecordRequest(
    Guid Id,
    string Diagnosis, string? Treatment,
    string? VetName, string? VetClinic,
    DateTime TreatmentDate, string? Notes);

public record GetHealthRecordsRequest(Guid AnimalInfoId);
public record GetHealthRecordRequest(Guid Id);
public record DeleteHealthRecordRequest(Guid Id);
