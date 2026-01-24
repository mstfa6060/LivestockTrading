namespace LivestockTrading.Application.RequestHandlers.Languages.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Code { get; set; }
	public string Name { get; set; }
	public string NativeName { get; set; }
	public bool IsRightToLeft { get; set; }
	public bool IsActive { get; set; }
	public bool IsDefault { get; set; }
	public int SortOrder { get; set; }
	public string FlagIconUrl { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
