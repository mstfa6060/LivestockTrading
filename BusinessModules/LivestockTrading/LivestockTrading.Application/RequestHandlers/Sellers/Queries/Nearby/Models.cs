namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Nearby;

public class RequestModel : IRequestModel
{
	/// <summary>Kullanicinin enlemi (WGS84)</summary>
	public double Latitude { get; set; }

	/// <summary>Kullanicinin boylami (WGS84)</summary>
	public double Longitude { get; set; }

	/// <summary>Opsiyonel ulke filtresi (ISO 3166-1 alpha-2). Bos birakilirsa tum ulkelerdeki saticilar dikkate alinir.</summary>
	public string CountryCode { get; set; }

	/// <summary>Dondurulecek maksimum sonuc sayisi (1-50). Varsayilan 10.</summary>
	public int Limit { get; set; } = 10;
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string BusinessName { get; set; }
	public string BusinessType { get; set; }
	public string LogoUrl { get; set; }
	public bool IsVerified { get; set; }
	public int Status { get; set; }
	public double? AverageRating { get; set; }
	public int TotalReviews { get; set; }
	public int TotalSales { get; set; }

	/// <summary>Saticinin birincil konumunun sehri</summary>
	public string City { get; set; }

	/// <summary>Saticinin birincil konumunun ulke kodu</summary>
	public string CountryCode { get; set; }

	/// <summary>Saticinin birincil konumunun enlemi</summary>
	public double Latitude { get; set; }

	/// <summary>Saticinin birincil konumunun boylami</summary>
	public double Longitude { get; set; }

	/// <summary>Kullanicidan kilometre cinsinden mesafe (Haversine)</summary>
	public double DistanceKm { get; set; }
}
