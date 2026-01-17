using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Common.Definitions.Domain.NonRelational.Entities;

public class FileBucket
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }

	public BucketTypes BucketType { get; set; }
	public bool IsWaitingForApproval { get; set; }
	public string ModuleName { get; set; }

	public List<FileEntry> Files { get; set; }
	public List<ChangeSet> ChangeSets { get; set; }
}

public enum BucketTypes
{
	SingleFileBucket,
	MultipleFileBucket,
}