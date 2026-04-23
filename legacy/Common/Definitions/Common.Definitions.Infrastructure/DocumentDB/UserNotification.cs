using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Common.Definitions.Base.Enums;
using System.Collections.Generic;

namespace Common.Definitions.Infrastructure.DocumentDB;

public class UserNotification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }

    [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; }

    public List<object> UnreadEntries { get; set; }
    public List<object> ReadEntries { get; set; }


}