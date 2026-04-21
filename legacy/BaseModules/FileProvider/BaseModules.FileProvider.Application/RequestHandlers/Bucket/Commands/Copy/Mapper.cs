using System.Collections.Immutable;

namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Commands.Copy;

public class Mapper
{
    public ResponseModel MapToResponse(FileBucket newBucket, Guid changeId)
    {
        return new ResponseModel()
        {
            BucketId = newBucket.Id,
            BucketType = newBucket.BucketType,
            ChangeId = changeId,
            Files = newBucket.Files.Select(fileEntry => new ResponseModel.FileResponse()
            {
                Id = fileEntry.Id,
                Name = fileEntry.Name,
                Path = fileEntry.Path,
                Extention = fileEntry.Extention,
                ContentType = fileEntry.ContentType,
                IsDefault = fileEntry.IsDefault,
                Index = fileEntry.Index,
            }).ToList(),
        };
    }
}