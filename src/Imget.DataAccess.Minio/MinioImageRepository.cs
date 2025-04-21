using Imget.DataAccess.Minio.Module.Configuration;
using Imget.DataAccess.Repositories;
using Microsoft.IO;
using Minio;
using Minio.DataModel.Args;

namespace Imget.DataAccess.Minio;

public class MinioImageRepository : IImageRepository
{
    private readonly IMinioClient _client;
    private readonly string _bucketName;
    private readonly string _prefix;

    private readonly RecyclableMemoryStreamManager _memoryManager = new();

    public MinioImageRepository(MinioImageConfig minioConfig)
    {
        _client = new MinioClient()
            .WithEndpoint(minioConfig.Endpoint)
            .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
            .Build();

        _bucketName = minioConfig.Bucket;
        _prefix = minioConfig.Prefix;
    }

    public async Task<Stream> GetAsync(string id, CancellationToken cancellationToken)
    {
        Stream resultStream = _memoryManager.GetStream();

        var args = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(GetFullPath(id))
            .WithCallbackStream((stream) =>
            {
                stream.CopyTo(resultStream);
                resultStream.Position = 0;
            });

        await _client.GetObjectAsync(args, cancellationToken);

        return resultStream;
    }

    public async Task SaveAsync(string id, Stream stream, CancellationToken cancellationToken)
    {
        using (Stream memStream = _memoryManager.GetStream())
        {
            await stream.CopyToAsync(memStream, cancellationToken);

            memStream.Position = 0;

            var args = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(GetFullPath(id))
                .WithStreamData(memStream)
                .WithObjectSize(memStream.Length);

            await _client.PutObjectAsync(args, CancellationToken.None);
        }
    }

    private string GetFullPath(string id)
    {
        return $"{_prefix}/{id}";
    }
}