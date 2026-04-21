namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Delete;

public class Mapper
{
    public ResponseModel MapToResponse(FileBucket bucket, FileEntry fileEntry, ChangeSet changeSet)
    {
        return new ResponseModel()
        {
            BucketId = bucket.Id,
            FileId = fileEntry.Id,
            ChangeId = changeSet.ChangeId,
        };
    }
}