namespace LivestockTrading.Application.RequestHandlers.Students.Commands.Delete;

public class RequestModel : IRequestModel
{
    public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}
