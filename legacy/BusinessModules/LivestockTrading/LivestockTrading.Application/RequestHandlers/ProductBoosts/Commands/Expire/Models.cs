namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Expire;

public class RequestModel : IRequestModel
{
}

public class ResponseModel : IResponseModel
{
    public int ExpiredCount { get; set; }
    public DateTime ProcessedAt { get; set; }
}
