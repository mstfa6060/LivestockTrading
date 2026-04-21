using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Files.Features.Services;

public sealed class MinioStorageService : IStorageService
{
    private readonly IMinioClient _minio;
    private readonly string _bucket;

    public MinioStorageService(IMinioClient minio, IOptions<MinioSettings> options)
    {
        _minio = minio;
        _bucket = options.Value.BucketName;
    }

    public async Task UploadAsync(Stream stream, string objectKey, string contentType, long sizeBytes, CancellationToken ct = default)
    {
        await EnsureBucketExistsAsync(ct);

        var args = new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey)
            .WithStreamData(stream)
            .WithObjectSize(sizeBytes)
            .WithContentType(contentType);

        await _minio.PutObjectAsync(args, ct);
    }

    public async Task DeleteAsync(string objectKey, CancellationToken ct = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey);

        await _minio.RemoveObjectAsync(args, ct);
    }

    public async Task<string> GetPresignedUrlAsync(string objectKey, int expirySeconds = 3600, CancellationToken ct = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey)
            .WithExpiry(expirySeconds);

        return await _minio.PresignedGetObjectAsync(args);
    }

    public async Task CopyAsync(string sourceKey, string destKey, CancellationToken ct = default)
    {
        var copySource = new CopySourceObjectArgs()
            .WithBucket(_bucket)
            .WithObject(sourceKey);

        var args = new CopyObjectArgs()
            .WithBucket(_bucket)
            .WithObject(destKey)
            .WithCopyObjectSource(copySource);

        await _minio.CopyObjectAsync(args, ct);
    }

    public async Task EnsureBucketExistsAsync(CancellationToken ct = default)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(_bucket);
        var exists = await _minio.BucketExistsAsync(existsArgs, ct);
        if (!exists)
        {
            var makeArgs = new MakeBucketArgs().WithBucket(_bucket);
            await _minio.MakeBucketAsync(makeArgs, ct);
        }
    }
}
