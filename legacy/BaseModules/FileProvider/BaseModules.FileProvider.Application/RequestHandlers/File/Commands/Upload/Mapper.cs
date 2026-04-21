namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Upload;

public class Mapper
{
	public ResponseModel MapToResponse(FileBucket bucket, List<FileEntry> visibleFileEntries, Guid changeId)
	{
		var mappedFileEnties = new List<ResponseModel.FileResponse>();

		visibleFileEntries.ForEach((fileEntry) =>
		{
			mappedFileEnties.Add(
							new ResponseModel.FileResponse()
							{
								Id = fileEntry.Id,
								Name = fileEntry.Name,
								Path = fileEntry.Path,
								Extention = fileEntry.Extention,
								ContentType = fileEntry.ContentType,
								IsDefault = fileEntry.IsDefault,
								Index = fileEntry.Index,
								Variants = fileEntry.Variants?.Select(v => new ResponseModel.ImageVariant
								{
									Key = v.Key,
									Url = v.Url
								}).ToList(),
								Width = fileEntry.Width,
								Height = fileEntry.Height,
								SizeBytes = fileEntry.SizeBytes,
							}
					);
		});

		return new ResponseModel()
		{
			BucketId = bucket.Id,
			ChangeId = changeId,
			BucketType = bucket.BucketType,
			ModuleName = bucket.ModuleName,
			Files = mappedFileEnties,
		};
	}
}