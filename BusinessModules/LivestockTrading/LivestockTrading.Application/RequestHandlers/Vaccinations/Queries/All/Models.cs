namespace LivestockTrading.Application.RequestHandlers.Vaccinations.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid AnimalInfoId { get; set; }
	public string VaccineName { get; set; }
	public string VaccineType { get; set; }
	public DateTime VaccinationDate { get; set; }
	public DateTime? NextDueDate { get; set; }
	public string VeterinarianName { get; set; }
	public DateTime CreatedAt { get; set; }
}
