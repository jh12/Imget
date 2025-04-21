using Imget.Shared.Configuration;

namespace Imget.DataAccess.Rmq.Module.Configuration;

public sealed record RebusConfig
(
    MinioDataBusConfig Minio,
    MongoDbConfig MongoDb
);