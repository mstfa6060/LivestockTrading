using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.AdminAssign;

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
		var actorUserId = _currentUserService.GetCurrentUserId();

		// Mevcut active aboneligi iptal et (yenisi tek aktif kalsin)
		await _dataAccessLayer.DeactivateExistingActiveSubscriptions(request.SellerId, cancellationToken);

		var entity = mapper.MapToEntity(request, actorUserId);
		await _dataAccessLayer.Add(entity, cancellationToken);
		await _dataAccessLayer.UpdateSellerActiveSubscription(request.SellerId, entity.Id, cancellationToken);
		await _dataAccessLayer.SaveChanges(cancellationToken);

		return ArfBlocksResults.Success(mapper.MapToResponse(entity));
	}
}
