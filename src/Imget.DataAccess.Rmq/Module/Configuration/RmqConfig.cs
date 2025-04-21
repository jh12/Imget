namespace Imget.DataAccess.Rmq.Module.Configuration;

public sealed record RmqConfig
(
    string ConnectionString,
    string InputQueueName
);