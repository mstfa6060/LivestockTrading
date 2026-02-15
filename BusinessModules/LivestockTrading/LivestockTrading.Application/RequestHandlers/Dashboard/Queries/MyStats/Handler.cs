namespace LivestockTrading.Application.RequestHandlers.Dashboard.Queries.MyStats;

/// <summary>
/// Dashboard Kişisel İstatistikleri (Gelişmiş)
/// Satıcının toplam ilan, aktif/taslak/bekleyen/satılmış ilan, görüntülenme, favori, mesaj,
/// okunmamış mesaj, yorum ve ortalama puan bilgilerini döner.
/// Ayrıca son aktiviteleri (mesaj, favori, yorum) listeler.
/// Period parametresi ile belirli zaman dilimlerine filtreleme yapılabilir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var req = (RequestModel)payload;

		var seller = await _dataAccessLayer.GetSellerByUserId(req.UserId, cancellationToken);
		if (seller == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.DashboardErrors.DashboardSellerNotFound));

		var stats = await _dataAccessLayer.GetStats(seller.Id, req.UserId, req.Period, cancellationToken);
		var recentActivity = await _dataAccessLayer.GetRecentActivity(seller.Id, req.UserId, req.Period, cancellationToken);

		var response = new ResponseModel
		{
			TotalListings = stats.TotalListings,
			ActiveListings = stats.ActiveListings,
			DraftListings = stats.DraftListings,
			PendingListings = stats.PendingListings,
			SoldListings = stats.SoldListings,
			TotalViews = stats.TotalViews,
			TotalFavorites = stats.TotalFavorites,
			TotalMessages = stats.TotalMessages,
			UnreadMessages = stats.UnreadMessages,
			TotalReviews = stats.TotalReviews,
			AverageRating = stats.AverageRating,
			RecentActivity = recentActivity
		};

		return ArfBlocksResults.Success(response);
	}
}
