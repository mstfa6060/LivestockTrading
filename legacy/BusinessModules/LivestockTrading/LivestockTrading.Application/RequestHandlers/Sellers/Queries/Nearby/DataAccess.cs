using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Nearby;

/// <summary>
/// "Yakinizdaki saticilar" icin seller + birincil konum (lat/lng) ciftlerini ceker.
/// Mesafe hesaplamasi Handler icinde Haversine ile bellek icinde yapilir.
/// </summary>
public class DataAccess : IDataAccess
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbContext = dependencyProvider.GetInstance<LivestockTradingModuleDbContext>();
	}

	/// <summary>
	/// Aktif ve dogrulanmis saticilarin ilk/son urunlerinin konumlarini cekip
	/// her satici icin (Seller, Lat, Lng, City, CountryCode) dondurur.
	/// countryCode verilirse sadece o ulkedeki konumlar dikkate alinir.
	/// </summary>
	public async Task<List<SellerLocation>> GetSellersWithPrimaryLocation(string countryCode, CancellationToken ct)
	{
		// Her seller icin latest product + location join'u. Koordinati olmayan konumlar elenir.
		var query = _dbContext.Products
			.AsNoTracking()
			.Where(p => !p.IsDeleted
				&& p.Status == ProductStatus.Active
				&& p.Location != null
				&& p.Location.Latitude != null
				&& p.Location.Longitude != null
				&& p.Seller != null
				&& !p.Seller.IsDeleted
				&& p.Seller.IsActive);

		if (!string.IsNullOrWhiteSpace(countryCode))
		{
			query = query.Where(p => p.Location.CountryCode == countryCode);
		}

		// EF Core'un cevirebildigi duz projeksiyon. Seller basina "en yeni urun"
		// seciminini bellek icinde yapacagiz (GroupBy+nested projection EF'de guvensiz).
		// Take(5000): buyuk dataset'te runaway query'i onle; 5k urun binlerce benzersiz saticiyi kapsar.
		var rows = await query
			.OrderByDescending(p => p.CreatedAt)
			.Take(5000)
			.Select(p => new SellerLocation
			{
				SellerId = p.SellerId,
				UserId = p.Seller.UserId,
				BusinessName = p.Seller.BusinessName,
				BusinessType = p.Seller.BusinessType,
				LogoUrl = p.Seller.LogoUrl,
				IsVerified = p.Seller.IsVerified,
				Status = (int)p.Seller.Status,
				AverageRating = p.Seller.AverageRating,
				TotalReviews = p.Seller.TotalReviews,
				TotalSales = p.Seller.TotalSales,
				City = p.Location.City,
				CountryCode = p.Location.CountryCode,
				Latitude = p.Location.Latitude.Value,
				Longitude = p.Location.Longitude.Value,
				ProductCreatedAt = p.CreatedAt
			})
			.ToListAsync(ct);

		// Seller basina en yeni urunun konumunu al (CreatedAt zaten desc sirali oldugu icin ilk gelen en yeni).
		return rows
			.GroupBy(r => r.SellerId)
			.Select(g => g.First())
			.ToList();
	}
}

/// <summary>
/// DataAccess'den Handler'a gecen veri projeksiyonu.
/// </summary>
public class SellerLocation
{
	public Guid SellerId { get; set; }
	public Guid UserId { get; set; }
	public string BusinessName { get; set; }
	public string BusinessType { get; set; }
	public string LogoUrl { get; set; }
	public bool IsVerified { get; set; }
	public int Status { get; set; }
	public double? AverageRating { get; set; }
	public int TotalReviews { get; set; }
	public int TotalSales { get; set; }
	public string City { get; set; }
	public string CountryCode { get; set; }
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public DateTime ProductCreatedAt { get; set; }
}
