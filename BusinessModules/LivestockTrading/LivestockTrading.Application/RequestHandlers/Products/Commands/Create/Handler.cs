using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		// Get seller ID - auto-create if not provided
		var sellerId = await GetOrCreateSellerId(request.SellerId, cancellationToken);

		var entity = mapper.MapToEntity(request);
		entity.SellerId = sellerId;

		await _dataAccessLayer.AddProduct(entity);

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}

	private async Task<Guid> GetOrCreateSellerId(Guid? requestSellerId, CancellationToken ct)
	{
		// If a valid SellerId is provided, use it
		if (requestSellerId.HasValue && requestSellerId.Value != Guid.Empty)
			return requestSellerId.Value;

		// Otherwise, get or create seller for current user
		var currentUserId = _currentUserService.GetCurrentUserId();
		var existingSeller = await _dataAccessLayer.GetSellerByUserId(currentUserId, ct);

		if (existingSeller != null)
			return existingSeller.Id;

		// Create new seller for the user
		var displayName = _currentUserService.GetCurrentUserDisplayName() ?? "Satıcı";
		var newSeller = new Seller
		{
			UserId = currentUserId,
			BusinessName = displayName,
			BusinessType = "Bireysel",
			IsActive = true,
			Status = SellerStatus.PendingVerification
		};

		await _dataAccessLayer.CreateSeller(newSeller, ct);
		return newSeller.Id;
	}
}
