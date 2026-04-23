namespace BaseModules.FileProvider.Application.EventHandlers.UserImages.Commands.CreateOrReplace;

[InternalHandler]
[EventHandler]
public class Handler : IRequestHandler
{
	private readonly IFileStorageService _fileStorageService;
	public Handler(ArfBlocksDependencyProvider dependencyProvider)
	{
		_fileStorageService = dependencyProvider.GetInstance<IFileStorageService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestPayload = (RequestModel)payload;
		var mapper = new Mapper();

		var uploadedFileProperties = await _fileStorageService.CreateUserImage(requestPayload.CompanyId, requestPayload.Name, requestPayload.Extention, requestPayload.Data);

		var response = mapper.MapToResponse(uploadedFileProperties);

		return ArfBlocksResults.Success(response);
	}
}
