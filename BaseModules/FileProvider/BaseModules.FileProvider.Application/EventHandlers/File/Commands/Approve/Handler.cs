namespace BaseModules.FileProvider.Application.EventHandlers.Files.Commands.Approve;

[InternalHandler]
[EventHandler]
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IFileStorageService _fileStorageService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_fileStorageService = dependencyProvider.GetInstance<IFileStorageService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var eventPayload = (RequestModel)payload;
		var mapper = new Mapper();

		//  Get Data
		var bucket = await _dataAccessLayer.GetBucketById(eventPayload.BucketId);

		// Get user's last changed TrackingSet
		var matchedTrackingSet = bucket.ChangeSets
										.Where(c => c.UserId == eventPayload.UserId)
										.OrderByDescending(c => c.LastChangedAt)
										.FirstOrDefault();

		if (matchedTrackingSet == null)
			return ArfBlocksResults.BadRequest("MATCHED_CHANGE_SET_NOT_FOUND_FOR_REQUESTED_USER");

		if (matchedTrackingSet.IsBucketCreated)
			bucket.IsWaitingForApproval = false;

		for (int i = 0; i < matchedTrackingSet.Items.Count; i++)
		{
			var changeItem = matchedTrackingSet.Items[i];
			var matchedFileEntry = bucket.Files?.FirstOrDefault(f => f.Id == changeItem.FileEntryId);

			if (matchedFileEntry == null)
				continue;

			var donotRemovePhysicalFile = false;
			if (changeItem.ChangeType == ChangeTypes.Deleted && i < matchedTrackingSet.Items.Count - 1)
			{
				var changeItemFileEntry = bucket.Files.FirstOrDefault(f => f.Id == changeItem.FileEntryId);

				for (int j = i + 1; j < matchedTrackingSet.Items.Count; j++)
				{
					var nextChangeItem = matchedTrackingSet.Items[j];
					var nextChangeItemFileEntry = bucket.Files.FirstOrDefault(f => f.Id == nextChangeItem.FileEntryId);


					if (changeItemFileEntry.Path == nextChangeItemFileEntry.Path)
					{
						donotRemovePhysicalFile = true;
						break;
					}
				}
			}

			switch (changeItem.ChangeType)
			{
				case ChangeTypes.Created:
					matchedFileEntry.IsWaitingForApproval = false;
					break;
				case ChangeTypes.Deleted:
					if (!donotRemovePhysicalFile)
						await _fileStorageService.DeleteFile(matchedFileEntry.Path);
					bucket.Files.Remove(matchedFileEntry);
					break;

				default:
					System.Console.WriteLine("UN_HANDLED_CHANGETYPES_ENUM");
					break;
			}
		}

		// For Consistency remove all other ChangeSet side-effects
		foreach (var fileEntry in bucket.Files)
		{
			// The files that created but not approved
			if (fileEntry.IsWaitingForApproval)
			{
				await _fileStorageService.DeleteFile(fileEntry.Path);

				// #Bug1
				// Do this operation outside of loop, it throws error: Collection was modified; enumeration operation may not execute.
				// bucket.Files.Remove(fileEntry);
			}
		}

		// #Bug1			
		bucket.Files.RemoveAll(f => f.IsWaitingForApproval);

		// Clear all ChangeSets
		bucket.ChangeSets = new List<ChangeSet>();

		// Update Bucket on Document Database
		await _dataAccessLayer.UpdateBucket(bucket);

		var response = mapper.MapToResponse(true);

		// Return Response
		return ArfBlocksResults.Success(response);
	}
}
