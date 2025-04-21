namespace Imget.DataAccess.Rmq.Module.Configuration;

public sealed record MongoDbConfig
(
    string ConnectionString,
    string DatabaseName
);