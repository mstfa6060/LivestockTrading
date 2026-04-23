using Microsoft.EntityFrameworkCore;
using Common.Definitions.Infrastructure.DocumentDB;
using MongoDB.Driver;
using Arfware.ArfBlocks.Core.Exceptions;
using Common.Services.ErrorCodeGenerator;
using Common.Definitions.Domain.NonRelational.Errors;

namespace BaseModules.FileProvider.Infrastructure.Services;

public class FileProviderDocumentDbValidationService
{
	private readonly DefinitionsDocumentDbContext _documentDbContext;
	public FileProviderDocumentDbValidationService(DefinitionsDocumentDbContext documentDbContext)
	{
		_documentDbContext = documentDbContext;
	}


	public async Task ValidateBucketExist(string bucketId)
	{
		var bucketExist = await _documentDbContext.FileBuckets
													.Find(b => b.Id == bucketId)
													.AnyAsync();

		if (!bucketExist)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.BucketErrors.FileBucketNotExist));
	}

	public async Task ValidateFileEntryExist(string bucketId, Guid entryId)
	{
		var bucket = await _documentDbContext.FileBuckets
													.Find(b => b.Id == bucketId)
													.FirstOrDefaultAsync();

		if (!bucket.Files.Any(f => f.Id == entryId))
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.BucketErrors.FileEntryNotExist));
	}
}