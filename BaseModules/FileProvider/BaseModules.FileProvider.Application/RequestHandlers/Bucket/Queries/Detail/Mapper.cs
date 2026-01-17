namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Queries.Detail;

public class Mapper
{
    public ResponseModel MapToResponse(FileBucket bucket, List<FileEntry> visibleFileEntries, Guid changeId)
    {
        var mappedFileEnties = new List<ResponseModel.FileResponse>();

        visibleFileEntries.ForEach((fileEntry) =>
        {
            mappedFileEnties.Add(new ResponseModel.FileResponse()
            {
                Id = fileEntry.Id,
                Name = fileEntry.Name,
                Path = fileEntry.Path,
                Extention = fileEntry.Extention,
                ContentType = fileEntry.ContentType,
                IsDefault = fileEntry.IsDefault,
                Index = fileEntry.Index,
            });
        });

        return new ResponseModel()
        {
            BucketId = bucket.Id,
            ChangeId = changeId,
            Files = mappedFileEnties,
        };
    }
}