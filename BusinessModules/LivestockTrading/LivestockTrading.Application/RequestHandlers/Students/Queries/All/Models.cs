namespace LivestockTrading.Application.RequestHandlers.Students.Queries.All;

public class RequestModel : IRequestModel
{
    public XSorting Sorting { get; set; }
    public List<XFilterItem> Filters { get; set; }
    public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
    public Guid Id { get; set; }
    public string StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Department { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
