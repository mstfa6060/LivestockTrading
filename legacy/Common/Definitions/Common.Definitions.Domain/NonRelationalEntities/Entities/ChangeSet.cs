using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Common.Definitions.Domain.NonRelational.Entities;

public class ChangeSet
{
	[BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
	public Guid ChangeId { get; set; }

	[BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
	public Guid UserId { get; set; }

	public DateTime LastChangedAt { get; set; }

	public bool IsBucketCreated { get; set; }
	public List<ChangeItem> Items { get; set; }
}

public class ChangeItem
{
	[BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
	public Guid FileEntryId { get; set; }

	public DateTime TimeStamp { get; set; }
	public ChangeTypes ChangeType { get; set; }
}

public enum ChangeTypes
{
	Created,
	Updated,
	Deleted,
}