namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.Pick;

public class RequestModel : IRequestModel
{
	public List<Guid> SelectedIds { get; set; }
	public string Keyword { get; set; }
	public int Limit { get; set; } = 10;
	/// <summary>
	/// Dil kodu (ISO 639-1, örn: "tr", "en", "de")
	/// Belirtilirse Name çevrilmiş olarak döner
	/// </summary>
	public string LanguageCode { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Slug { get; set; }
}
