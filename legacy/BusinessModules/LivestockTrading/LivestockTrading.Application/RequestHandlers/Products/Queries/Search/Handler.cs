using LivestockTrading.Application.Authorization;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Search;

/// <summary>
/// Ürün Arama
/// Anahtar kelime, kategori, fiyat aralığı, konum ve diğer filtrelere göre ürün arar.
/// Admin/Moderator için silinmiş ürünler de dahildir. Diğerleri için sadece aktif (Status=Active) ürünler döner.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly PermissionService _permissionService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var req = (RequestModel)payload;
		var includeDeleted = _permissionService.IsModerator();

		var (products, page) = await _dataAccessLayer.Search(
			req.Query,
			req.CategoryId,
			req.BrandId,
			req.MinPrice,
			req.MaxPrice,
			req.Condition,
			req.CountryCode,
			req.City,
			req.SellerId,
			req.Sorting,
			req.PageRequest,
			includeDeleted,
			cancellationToken);

		// ViewerCurrencyCode verilmişse pre-computed ProductPrice kayıtlarını al
		Dictionary<Guid, LivestockTrading.Domain.Entities.ProductPrice> viewerPrices = null;
		string viewerCurrencySymbol = null;
		if (!string.IsNullOrWhiteSpace(req.ViewerCurrencyCode) && products.Any())
		{
			var productIds = products.Select(p => p.Id).ToList();
			viewerPrices = await _dataAccessLayer.GetProductPricesForCurrency(productIds, req.ViewerCurrencyCode, cancellationToken);
			viewerCurrencySymbol = await _dataAccessLayer.GetCurrencySymbol(req.ViewerCurrencyCode, cancellationToken);
		}

		var response = mapper.MapToResponse(products, viewerPrices, req.ViewerCurrencyCode, viewerCurrencySymbol);
		return ArfBlocksResults.Success(response, page);
	}
}
