namespace BaseModules.FileProvider.Application.EventHandlers.Files.Commands.Approve;

public class DataAccess : IDataAccess
{
	private readonly DefinitionsDocumentDbContext _documentDbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_documentDbContext = dependencyProvider.GetInstance<DefinitionsDocumentDbContext>();
	}

	public async Task<FileBucket> GetBucketById(string bucketId)
	{
		return await _documentDbContext.FileBuckets
											.Find(v => v.Id == bucketId).FirstOrDefaultAsync();
	}

	public async Task UpdateBucket(FileBucket bucket)
	{
		await _documentDbContext.FileBuckets
									.FindOneAndReplaceAsync(v => v.Id == bucket.Id, bucket);
	}

	public async Task DeleteBucket(FileBucket bucket)
	{
		await _documentDbContext.FileBuckets
									.DeleteOneAsync(v => v.Id == bucket.Id);
	}
}
