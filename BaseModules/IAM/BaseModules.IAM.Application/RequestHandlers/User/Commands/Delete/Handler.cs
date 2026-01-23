namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Delete;

/// <summary>
/// Kullanıcı Silme
/// Bu endpoint, bir kullanıcı hesabını siler.
/// İlişkili tüm veriler temizlenir.
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
		{
			var request = (RequestModel)payload;
			var mapper = new Mapper();

			var user = await _dataAccessLayer.GetUserById(request.UserId);

			user.IsDeleted = !request.IsDeleted;

			await _dataAccessLayer.DeleteUser(user);

			var response = mapper.MapToResponse(user);

			return ArfBlocksResults.Success(response);
		}
	}
}
