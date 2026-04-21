namespace LivestockTrading.Application.RequestHandlers.BoostPackages.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string NameTranslations { get; set; }
	public string DescriptionTranslations { get; set; }
	public int DurationHours { get; set; }
	public double Price { get; set; }
	public string Currency { get; set; }
	public int BoostType { get; set; }
	public int BoostScore { get; set; }
	public string AppleProductId { get; set; }
	public string GoogleProductId { get; set; }
	public bool IsActive { get; set; }
	public int SortOrder { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string NameTranslations { get; set; }
	public string DescriptionTranslations { get; set; }
	public int DurationHours { get; set; }
	public double Price { get; set; }
	public string Currency { get; set; }
	public int BoostType { get; set; }
	public int BoostScore { get; set; }
	public string AppleProductId { get; set; }
	public string GoogleProductId { get; set; }
	public bool IsActive { get; set; }
	public int SortOrder { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
