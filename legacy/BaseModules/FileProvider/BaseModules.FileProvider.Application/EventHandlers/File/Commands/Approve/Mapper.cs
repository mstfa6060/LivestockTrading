namespace BaseModules.FileProvider.Application.EventHandlers.Files.Commands.Approve;

public class Mapper
{
    public ResponseModel MapToResponse(bool isEverythingOk)
    {
        return new ResponseModel()
        {
            IsEverythingOk = isEverythingOk,
        };
    }
}
