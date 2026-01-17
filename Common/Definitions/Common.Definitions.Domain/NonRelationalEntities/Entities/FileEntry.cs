using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Definitions.Domain.NonRelational.Entities;

public class FileEntry
{
	[BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
	public Guid Id { get; set; }

	public bool IsWaitingForApproval { get; set; }

	// Relation for Relational DB User Entity
	[BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
	public Guid UserId { get; set; }
	public string UserDisplayName { get; set; }

	public bool IsDefault { get; set; }
	public int Index { get; set; }
	public DateTime UploadedAt { get; set; }

	public string Extention { get; set; }
	public string Name { get; set; }
	public string Path { get; set; }
	public string ContentType { get; set; }
}