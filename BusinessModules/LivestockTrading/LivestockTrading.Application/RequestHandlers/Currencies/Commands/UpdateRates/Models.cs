namespace LivestockTrading.Application.RequestHandlers.Currencies.Commands.UpdateRates;

public class RequestModel : IRequestModel
{
	/// <summary>Güncellenecek kur bilgileri listesi</summary>
	public List<CurrencyRateItem> Rates { get; set; }
}

public class CurrencyRateItem
{
	/// <summary>Para birimi kodu (ISO 4217)</summary>
	public string Code { get; set; }

	/// <summary>USD'ye göre dönüşüm kuru</summary>
	public decimal ExchangeRateToUSD { get; set; }
}

public class ResponseModel : IResponseModel
{
	/// <summary>Güncellenen para birimi sayısı</summary>
	public int UpdatedCount { get; set; }

	/// <summary>Güncelleme zamanı</summary>
	public DateTime UpdatedAt { get; set; }
}
