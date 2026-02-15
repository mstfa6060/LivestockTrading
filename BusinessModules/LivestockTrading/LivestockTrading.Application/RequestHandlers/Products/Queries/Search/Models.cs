namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Search;

public class RequestModel : IRequestModel
{
	/// <summary>
	/// Arama sorgusu - ürün başlığı, açıklaması, slug'ında aranır
	/// </summary>
	public string Query { get; set; }

	/// <summary>
	/// Kategori filtresi (opsiyonel)
	/// </summary>
	public Guid? CategoryId { get; set; }

	/// <summary>
	/// Marka filtresi (opsiyonel)
	/// </summary>
	public Guid? BrandId { get; set; }

	/// <summary>
	/// Minimum fiyat filtresi (opsiyonel)
	/// </summary>
	public decimal? MinPrice { get; set; }

	/// <summary>
	/// Maksimum fiyat filtresi (opsiyonel)
	/// </summary>
	public decimal? MaxPrice { get; set; }

	/// <summary>
	/// Ürün durumu filtresi (opsiyonel, enum int değeri)
	/// New=0, Used=1, Refurbished=2, ForParts=3
	/// </summary>
	public int? Condition { get; set; }

	/// <summary>
	/// Ülke kodu filtresi (ISO 3166-1 alpha-2, örn: "TR", "US")
	/// </summary>
	public string CountryCode { get; set; }

	/// <summary>
	/// Şehir filtresi (opsiyonel)
	/// </summary>
	public string City { get; set; }

	/// <summary>
	/// Satıcı ID filtresi (opsiyonel)
	/// </summary>
	public Guid? SellerId { get; set; }

	/// <summary>
	/// Para birimi filtresi (opsiyonel, örn: "USD", "EUR", "TRY")
	/// </summary>
	public string Currency { get; set; }

	/// <summary>
	/// Sıralama kriteri: "relevance", "price_asc", "price_desc", "newest", "most_viewed"
	/// </summary>
	public string SortBy { get; set; }

	public XSorting Sorting { get; set; }
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
	public string CategoryName { get; set; }
	public string SellerName { get; set; }
	public Guid LocationId { get; set; }
	public string LocationCountryCode { get; set; }
	public string LocationCity { get; set; }
	public int Status { get; set; }
	public int Condition { get; set; }
	public int ViewCount { get; set; }
	public int FavoriteCount { get; set; }
	public decimal? AverageRating { get; set; }
	public int ReviewCount { get; set; }
	public string CoverImageFileId { get; set; }
	public string MediaBucketId { get; set; }
	public DateTime CreatedAt { get; set; }
}
