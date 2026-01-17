
namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Create;

/// <summary>
/// Rol Oluşturma
/// Bu endpoint, sistemde yeni bir kullanıcı rolü oluşturur.
/// Yetki ve izinler tanımlanır.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly ArfBlocksCommunicator _communicator;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_communicator = dependencyProvider.GetInstance<ArfBlocksCommunicator>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var requestPayload = (RequestModel)payload;

		// Map Request Payload to Entity
		var role = mapper.MapToNewEntity(requestPayload);

		// Add To DB
		await _dataAccessLayer.AddRole(role);

		var response = mapper.MapToResponse(role);
		return ArfBlocksResults.Success(response);
	}

}