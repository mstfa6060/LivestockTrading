namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Search;

/// <summary>
/// Ürün Arama
/// Anahtar kelime, kategori, fiyat aralığı, konum ve diğer filtrelere göre ürün arar.
/// Sadece aktif (Status=Active) ürünler sonuçlarda gösterilir.
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
			req.Currency,
			req.SortBy,
			req.Sorting,
			req.PageRequest,
			cancellationToken);

		var response = mapper.MapToResponse(products);
		return ArfBlocksResults.Success(response, page);
	}
}
