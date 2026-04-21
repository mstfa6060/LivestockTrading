namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Queries.Detail;

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
}