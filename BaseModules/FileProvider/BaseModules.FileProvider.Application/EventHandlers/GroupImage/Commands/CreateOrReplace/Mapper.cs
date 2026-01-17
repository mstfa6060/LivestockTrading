namespace BaseModules.FileProvider.Application.EventHandlers.GroupImages.Commands.CreateOrReplace;

public class Mapper
{
    public ResponseModel MapToResponse(FileProperties fileProperties)
    {
        return new ResponseModel()
        {
            ContentType = fileProperties.ContentType,
            Extention = fileProperties.Extention,
            Name = fileProperties.Name,
            Path = fileProperties.Path,
        };
    }
}
