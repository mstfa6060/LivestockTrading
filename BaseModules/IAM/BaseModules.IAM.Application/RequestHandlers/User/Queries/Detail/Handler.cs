namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.Detail;

/// <summary>
/// Kullanıcı Detayı
/// Bu endpoint, belirli bir kullanıcının detaylı bilgilerini getirir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
	{
		_dataAccessLayer = dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var user = await _dataAccessLayer.GetUser(request.UserId, request.CompanyId);

		if (user == null)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));

		var response = mapper.MapToResponse(user);
		return ArfBlocksResults.Success(response);
	}
}
