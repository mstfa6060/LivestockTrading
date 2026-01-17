using System;
using MongoDB.Driver;
using System.Linq;
using Common.Definitions.Domain.NonRelational.Entities;
using System.Collections.Generic;

namespace BaseModules.FileProvider.Infrastructure.Services;

public class FileChangeTrackingService
{
	public FileChangeTrackingService()
	{ }

	public List<FileEntry> GetVisibleFileEntries(FileBucket bucket, Guid changeId)
	{
		var fileEntries = new List<FileEntry>();
		var changeSet = bucket.ChangeSets?.FirstOrDefault(c => c.ChangeId == changeId);

		bucket.Files?.ForEach((fileEntry) =>
		{
			// Must use LastOrDefault, Because A file can be created then can be deleted. 
			// If use FirstOrDefault; file will be recognized as creted but file last state is not creted it is deleted.
			var fileChangeItem = changeSet?.Items?.LastOrDefault(i => i.FileEntryId == fileEntry.Id);
			var isFileCreatedButNotApproved = fileEntry.IsWaitingForApproval;

			// Skip any file that created but not approved
			if (isFileCreatedButNotApproved && (fileChangeItem == null || fileChangeItem?.ChangeType != ChangeTypes.Created))
				return;

			// skip any file that deleted but not approved
			if (fileChangeItem != null && fileChangeItem.ChangeType == ChangeTypes.Deleted)
				return;

			fileEntries.Add(fileEntry);
		});

		return fileEntries;
	}

	public ChangeSet AddFileCreating(FileBucket bucket, FileEntry fileEntry, Guid? changeId, Guid currentClientId)
	{
		return this.CreateChangeItem(bucket, fileEntry, changeId, currentClientId, ChangeTypes.Created);
	}

	public ChangeSet AddFileDeleting(FileBucket bucket, FileEntry fileEntry, Guid? changeId, Guid currentClientId)
	{
		return this.CreateChangeItem(bucket, fileEntry, changeId, currentClientId, ChangeTypes.Deleted);
	}

	private ChangeSet CreateChangeItem(FileBucket bucket, FileEntry fileEntry, Guid? changeId, Guid currentClientId, ChangeTypes changeType)
	{
		ChangeSet changeSet = null;

		if (changeId.HasValue)
			changeSet = bucket.ChangeSets?.FirstOrDefault(c => c.ChangeId == changeId.Value);

		if (changeSet == null)
		{
			// Create new ChangeSet
			changeSet = new ChangeSet()
			{
				ChangeId = changeId.HasValue ? changeId.Value : Guid.NewGuid(),
				UserId = currentClientId,
				IsBucketCreated = (changeType == ChangeTypes.Created) ? bucket.IsWaitingForApproval : false,
				Items = new List<ChangeItem>(),
			};

			// Null Check
			if (bucket.ChangeSets == null)
				bucket.ChangeSets = new List<ChangeSet>();

			// Add new ChangeSet
			bucket.ChangeSets.Add(changeSet);
		}

		// Null Check
		if (changeSet.Items == null)
			changeSet.Items = new List<ChangeItem>();

		// Set LastChangedAt property
		changeSet.LastChangedAt = DateTime.UtcNow;

		// Create Change Item
		changeSet.Items.Add(new ChangeItem()
		{
			FileEntryId = fileEntry.Id,
			ChangeType = changeType,
			TimeStamp = DateTime.UtcNow,
		});

		return changeSet;
	}
}