namespace LivestockTrading.Application.RequestHandlers.Products.Queries.All;

public class RequestModel : IRequestModel
{
	/// <summary>
	/// Ülke kodu filtresi (ISO 3166-1 alpha-2, örn: "TR", "US", "DE")
	/// Belirtilirse sadece bu ülkedeki ürünler döner
	/// </summary>
	public string CountryCode { get; set; }
	/// <summary>
	/// Kategori filtresi. Üst kategori verilirse alt kategorilerin ürünleri de dahil edilir.
	/// </summary>
	public Guid? CategoryId { get; set; }
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Slug { get; set; }
	public string ShortDescription { get; set; }
	public Guid CategoryId { get; set; }
	public Guid? BrandId { get; set; }
	public decimal BasePrice { get; set; }
	public string Currency { get; set; }
	public decimal? DiscountedPrice { get; set; }
	public int StockQuantity { get; set; }
	public bool IsInStock { get; set; }
	public Guid SellerId { get; set; }
	public Guid LocationId { get; set; }
	/// <summary>Ürünün bulunduğu ülke kodu (ISO 3166-1 alpha-2)</summary>
	public string LocationCountryCode { get; set; }
	/// <summary>Ürünün bulunduğu şehir</summary>
	public string LocationCity { get; set; }
	public int Status { get; set; }
	public int Condition { get; set; }
	public int ViewCount { get; set; }
	public decimal? AverageRating { get; set; }
	public int ReviewCount { get; set; }
	public DateTime CreatedAt { get; set; }
	public string MediaBucketId { get; set; }
	public string CoverImageFileId { get; set; }
}
