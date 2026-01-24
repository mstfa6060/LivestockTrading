namespace LivestockTrading.Application.RequestHandlers.Vaccinations.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public Guid AnimalInfoId { get; set; }
	public string VaccineName { get; set; }
	public string VaccineType { get; set; }
	public string BatchNumber { get; set; }
	public DateTime VaccinationDate { get; set; }
	public DateTime? NextDueDate { get; set; }
	public string VeterinarianName { get; set; }
	public string VeterinarianLicense { get; set; }
	public string Notes { get; set; }
	public string CertificateUrl { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid AnimalInfoId { get; set; }
	public string VaccineName { get; set; }
	public string VaccineType { get; set; }
	public string BatchNumber { get; set; }
	public DateTime VaccinationDate { get; set; }
	public DateTime? NextDueDate { get; set; }
	public string VeterinarianName { get; set; }
	public string VeterinarianLicense { get; set; }
	public string Notes { get; set; }
	public string CertificateUrl { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
