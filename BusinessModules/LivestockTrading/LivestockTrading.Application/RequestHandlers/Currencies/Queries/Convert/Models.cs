namespace LivestockTrading.Application.RequestHandlers.Currencies.Queries.Convert;

public class RequestModel : IRequestModel
{
	/// <summary>Dönüştürülecek tutar</summary>
	public decimal Amount { get; set; }

	/// <summary>Kaynak para birimi kodu (ISO 4217, örn: "USD", "EUR", "TRY")</summary>
	public string FromCurrencyCode { get; set; }

	/// <summary>Hedef para birimi kodu (ISO 4217, örn: "TRY", "EUR", "GBP")</summary>
	public string ToCurrencyCode { get; set; }
}

public class ResponseModel : IResponseModel
{
	/// <summary>Orijinal tutar</summary>
	public decimal OriginalAmount { get; set; }

	/// <summary>Dönüştürülmüş tutar</summary>
	public decimal ConvertedAmount { get; set; }

	/// <summary>Kaynak para birimi kodu</summary>
	public string FromCurrencyCode { get; set; }

	/// <summary>Hedef para birimi kodu</summary>
	public string ToCurrencyCode { get; set; }

	/// <summary>Uygulanan dönüşüm kuru (from → to)</summary>
	public decimal ExchangeRate { get; set; }

	/// <summary>Kaynak para birimi sembolü</summary>
	public string FromSymbol { get; set; }

	/// <summary>Hedef para birimi sembolü</summary>
	public string ToSymbol { get; set; }

	/// <summary>Kurların son güncellenme zamanı</summary>
	public DateTime LastUpdated { get; set; }
}
