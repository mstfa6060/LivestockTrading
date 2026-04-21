namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Commands.Duplicate;

public class Mapper
{
    public ResponseModel MapToResponse(FileBucket newBucket, Guid changeId)
    {
        return new ResponseModel()
        {
            BucketId = newBucket.Id,
            BucketType = newBucket.BucketType,
            ChangeId = changeId,
        };
    }
}