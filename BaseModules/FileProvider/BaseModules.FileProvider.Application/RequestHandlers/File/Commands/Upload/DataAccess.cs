namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Upload;

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
		{
			await _documentDbContext.FileBuckets.InsertOneAsync(bucket);
			return;
		}

		// Upload Handler artik bucket.Id'yi (Mongo ObjectId) yazimdan once uretiyor; bu durumda
		// kayit henuz Mongo'da yok. Upsert olmadan FindOneAndReplaceAsync no-op olur ve bucket
		// kalici olmaz; sonraki upload ayni Id'yi bulamayinca yeni bucket aciyor.
		await _documentDbContext.FileBuckets.FindOneAndReplaceAsync(
			v => v.Id == bucket.Id,
			bucket,
			new FindOneAndReplaceOptions<FileBucket> { IsUpsert = true });
	}
}