using LivestockTrading.Domain.Errors;
using Common.Services.ErrorCodeGenerator;
using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Update;

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

		var entity = await _dataAccessLayer.GetById(request.Id);

		if (entity == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductReportErrors.ProductReportNotFound));

		var currentUserId = _currentUserService.GetCurrentUserId();
		mapper.MapToEntity(request, entity, currentUserId);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
