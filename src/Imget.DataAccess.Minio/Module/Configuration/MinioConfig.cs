namespace Imget.DataAccess.Minio.Module.Configuration;

public sealed record MinioImageConfig
(
    string Endpoint,
    string AccessKey,
    string SecretKey,
    string Bucket,
    string Prefix
);