using System;
using MongoDB.Driver;
using Common.Definitions.Domain.NonRelational.Entities;

namespace Common.Definitions.Infrastructure.DocumentDB;

public class DefinitionsDocumentDbContext
{
    public DefinitionsDocumentDbContext(DocumentDbOptions options)
    {
        var client = new MongoClient(options.ConnectionString);
        var database = client.GetDatabase(options.DatabaseName);
        this.FileBuckets = database.GetCollection<FileBucket>(FileBucketCollectionName);
        this.Notifications = database.GetCollection<UserNotification>(NotificationCollectionName);
    }

    private string FileBucketCollectionName = "file_buckets";
    public IMongoCollection<FileBucket> FileBuckets;

    private string NotificationCollectionName = "notification";
    public IMongoCollection<UserNotification> Notifications;
}