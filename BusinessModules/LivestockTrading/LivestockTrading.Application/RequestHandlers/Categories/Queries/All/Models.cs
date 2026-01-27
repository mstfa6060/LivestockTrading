namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.All;

public class RequestModel : IRequestModel
{
	/// <summary>
	/// Dil kodu (ISO 639-1, örn: "tr", "en", "de")
	/// Belirtilirse Name ve Description çevrilmiş olarak döner
	/// </summary>
	public string LanguageCode { get; set; }
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Slug { get; set; }
	public string Description { get; set; }
	public string IconUrl { get; set; }
	public int SortOrder { get; set; }
	public bool IsActive { get; set; }
	public Guid? ParentCategoryId { get; set; }
	public string NameTranslations { get; set; }
	public string DescriptionTranslations { get; set; }
	public string AttributesTemplate { get; set; }
	public int SubCategoryCount { get; set; }
	public DateTime CreatedAt { get; set; }
}
