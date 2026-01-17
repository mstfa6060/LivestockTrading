using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Common.Definitions.Base.Enums;
using System.Collections.Generic;

namespace Common.Definitions.Infrastructure.DocumentDB;

public class NotificationEntry
{
    [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
    public Guid EntryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public NotificationStatus Status { get; set; }
    public UserModules Module { get; set; }

    // public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

}

public enum NotificationStatus
{
    Unread = 0,
    Read = 1,
}

// public enum NotificationType
// {
//     Mail = 0,
//     Sms = 1,
//     PushNotification = 2,
// }