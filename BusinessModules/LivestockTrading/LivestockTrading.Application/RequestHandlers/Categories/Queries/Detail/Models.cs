namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Slug { get; set; }
	public string Description { get; set; }
	public string IconUrl { get; set; }
	public int SortOrder { get; set; }
	public bool IsActive { get; set; }
	public Guid? ParentCategoryId { get; set; }
	public string ParentCategoryName { get; set; }
	public string NameTranslations { get; set; }
	public string DescriptionTranslations { get; set; }
	public string AttributesTemplate { get; set; }
	public List<SubCategoryItem> SubCategories { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public class SubCategoryItem
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string IconUrl { get; set; }
		public int SortOrder { get; set; }
		public bool IsActive { get; set; }
	}
}
