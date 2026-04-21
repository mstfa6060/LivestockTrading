namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadEyp;

public class Mapper
{
	public ResponseModel MapToResponse(Guid entityId, FileProperties fileProperties)
	{
		return new ResponseModel()
		{
			EntityId = entityId,
			Name = fileProperties.Name,
			Path = fileProperties.Path,
			Extention = fileProperties.Extention,
			ContentType = fileProperties.ContentType,
		};
	}
}