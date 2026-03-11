using LivestockTrading.Application.Authorization;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Detail;

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

		var product = await _dataAccessLayer.GetById(req.Id, includeDeleted, cancellationToken);
		if (product == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		var response = mapper.MapToResponse(product);

		return ArfBlocksResults.Success(response);
	}
}
