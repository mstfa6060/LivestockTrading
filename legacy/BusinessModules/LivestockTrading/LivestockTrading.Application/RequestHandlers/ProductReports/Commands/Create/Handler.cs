using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Create;

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

		var currentUserId = _currentUserService.GetCurrentUserId();

		var entity = mapper.MapToEntity(request, currentUserId);

		await _dataAccessLayer.Add(entity);

		var response = new ResponseModel { Id = entity.Id, Success = true };
		return ArfBlocksResults.Success(response);
	}
}
