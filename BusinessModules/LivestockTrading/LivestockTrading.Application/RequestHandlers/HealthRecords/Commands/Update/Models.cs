namespace LivestockTrading.Application.RequestHandlers.HealthRecords.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid AnimalInfoId { get; set; }
	public DateTime RecordDate { get; set; }
	public string RecordType { get; set; }
	public string VeterinarianName { get; set; }
	public string VeterinarianLicense { get; set; }
	public string ClinicName { get; set; }
	public string Diagnosis { get; set; }
	public string Treatment { get; set; }
	public string Medications { get; set; }
	public string Notes { get; set; }
	public DateTime? FollowUpDate { get; set; }
	public string DocumentUrl { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid AnimalInfoId { get; set; }
	public DateTime RecordDate { get; set; }
	public string RecordType { get; set; }
	public string VeterinarianName { get; set; }
	public string VeterinarianLicense { get; set; }
	public string ClinicName { get; set; }
	public string Diagnosis { get; set; }
	public string Treatment { get; set; }
	public string Medications { get; set; }
	public string Notes { get; set; }
	public DateTime? FollowUpDate { get; set; }
	public string DocumentUrl { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
