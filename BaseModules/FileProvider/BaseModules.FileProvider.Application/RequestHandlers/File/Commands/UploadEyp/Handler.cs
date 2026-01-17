using Common.Services.Auth.CurrentUser;

namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadEyp;

/// <summary>
/// EYP Dosya Yükleme
/// Bu endpoint, EYP formatında dosya yükler.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;
	private readonly IFileStorageService _fileStorageService;
	private readonly FileChangeTrackingService _fileChangeTrackingService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_fileStorageService = dependencyProvider.GetInstance<IFileStorageService>();
		_fileChangeTrackingService = dependencyProvider.GetInstance<FileChangeTrackingService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestPayload = (RequestModel)payload;
		var mapper = new Mapper();

		// Create File as Physical 
		// TODO: Make control for eyp file existence
		var moduleName = "ebys";
		var folderName = "documents";
		var documentId = requestPayload.EntityId;
		var versionName = "1";
		var eypFileName = $"{documentId.ToString()}.{requestPayload.FileExtention}";
		var tenantId = Guid.Empty;

		var uploadedFileProperties = await _fileStorageService.CreateEypOfPdfFileByFormFile(tenantId, requestPayload.FormFile, eypFileName, moduleName, folderName, requestPayload.EntityId, versionName);
		var response = mapper.MapToResponse(requestPayload.EntityId, uploadedFileProperties);

		return ArfBlocksResults.Success(response);
	}
}