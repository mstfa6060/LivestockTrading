namespace LivestockTrading.Application.RequestHandlers.Students.Queries.Detail;

public class RequestModel : IRequestModel
{
    public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
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
    public DateTime? UpdatedAt { get; set; }
}
