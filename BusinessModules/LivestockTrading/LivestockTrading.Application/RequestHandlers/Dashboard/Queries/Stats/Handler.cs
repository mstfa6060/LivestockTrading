namespace LivestockTrading.Application.RequestHandlers.Dashboard.Queries.Stats;

/// <summary>
/// Dashboard İstatistikleri
/// Satıcının toplam ilan, aktif ilan, görüntülenme, favori, mesaj, satış ve gelir bilgilerini döner.
/// Ayrıca son aktiviteleri listeler.
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
		var mapper = new Mapper();
		var req = (RequestModel)payload;

		var seller = await _dataAccessLayer.GetSellerByUserId(req.UserId, cancellationToken);
		if (seller == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.DashboardErrors.DashboardSellerNotFound));

		var totalListings = await _dataAccessLayer.GetTotalListings(seller.Id, cancellationToken);
		var activeListings = await _dataAccessLayer.GetActiveListings(seller.Id, cancellationToken);
		var totalViews = await _dataAccessLayer.GetTotalViews(seller.Id, cancellationToken);
		var totalFavorites = await _dataAccessLayer.GetTotalFavorites(seller.Id, cancellationToken);
		var totalMessages = await _dataAccessLayer.GetTotalMessages(req.UserId, cancellationToken);

		var recentProducts = await _dataAccessLayer.GetRecentProducts(seller.Id, 5, cancellationToken);
		var recentMessages = await _dataAccessLayer.GetRecentMessages(req.UserId, 5, cancellationToken);

		var response = mapper.MapToResponse(
			seller, totalListings, activeListings, totalViews, totalFavorites,
			totalMessages, recentProducts, recentMessages);

		return ArfBlocksResults.Success(response);
	}
}
