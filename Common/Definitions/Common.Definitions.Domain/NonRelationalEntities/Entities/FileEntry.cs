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

	// Resim varyantlari icin
	/// <summary>
	/// Resim varyant path'leri: { "thumb": "path/thumb.webp", "medium": "...", "large": "..." }
	/// </summary>
	public Dictionary<string, string> Variants { get; set; }

	/// <summary>
	/// Resim genisligi (px)
	/// </summary>
	public int? Width { get; set; }

	/// <summary>
	/// Resim yuksekligi (px)
	/// </summary>
	public int? Height { get; set; }

	/// <summary>
	/// Dosya boyutu (bytes)
	/// </summary>
	public long? SizeBytes { get; set; }
}