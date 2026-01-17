namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadToApprovedBucket;

public class DataAccess : IDataAccess
{
	private readonly DefinitionsDocumentDbContext _documentDbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_documentDbContext = dependencyProvider.GetInstance<DefinitionsDocumentDbContext>();
	}

	public async Task<FileBucket> GetBucketById(string bucketId)
	{
		return await _documentDbContext.FileBuckets.Find(v => v.Id == bucketId).FirstOrDefaultAsync();
	}

	public async Task CreateOrUpdateBucket(FileBucket bucket)
	{
		if (string.IsNullOrEmpty(bucket.Id))
			await _documentDbContext.FileBuckets.InsertOneAsync(bucket);
		else
			await _documentDbContext.FileBuckets.FindOneAndReplaceAsync(v => v.Id == bucket.Id, bucket);
	}
}