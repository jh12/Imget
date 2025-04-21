using System.Text.Json;
using Autofac;
using Imget.DataAccess.Rmq.Module.Configuration;
using Imget.Shared.Configuration;
using Imget.Shared.DataBus.Minio;
using MongoDB.Driver;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Serialization.Json;
using Serilog;
using RmqConfig = Imget.DataAccess.Rmq.Module.Configuration.RmqConfig;

namespace Imget.DataAccess.Rmq.Module;

public class RmqModule : Autofac.Module
{
    private readonly ILogger _logger;

    public RmqModule(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override void Load(ContainerBuilder builder)
    {
        RegisterMongo(builder);
        RegisterRebus(builder);
        RegisterHandlers(builder);
    }

    // TODO: Move to proper place
    private static void RegisterMongo(ContainerBuilder builder)
    {
        builder.Register(ctx =>
        {
            MongoDbConfig config = ctx.Resolve<MongoDbConfig>();

            MongoClient mongoClient = new(config.ConnectionString);
            return mongoClient.GetDatabase(config.DatabaseName);
        }).SingleInstance();
    }

    private void RegisterRebus(ContainerBuilder builder)
    {
        JsonSerializerOptions serializerOptions = new();

        builder.RegisterRebus((cfg, ctx) => cfg
                .Logging(l => l.Serilog(_logger))
                .Transport(t =>
                {
                    RmqConfig config = ctx.Resolve<RmqConfig>();
                    t.UseRabbitMq(config.ConnectionString, config.InputQueueName)
                        .InputQueueOptions(q => q.AddArgument("x-queue-type", "quorum"))
                        .DefaultQueueOptions(q => q.AddArgument("x-queue-type", "quorum"));
                })
                .Sagas(s =>
                {
                    IMongoDatabase database = ctx.Resolve<IMongoDatabase>();

                    s.StoreInMongoDb(database, _ => "sagas");
                })
                .Serialization(s => s.UseSystemTextJson(serializerOptions))
                .Routing(r => r.TypeBased())
                .DataBus(d =>
                {
                    d.UseMinioBlobStorage(ctx.Resolve<MinioDataBusConfig>());
                })
                .Options(o =>
                {
                    o.SetNumberOfWorkers(2);
                    o.SetMaxParallelism(30);
                })
            , startAutomatically: true);
    }

    private static void RegisterHandlers(ContainerBuilder builder)
    {
        builder.RegisterHandlersFromAssemblyOf<RmqModule>();
    }
}