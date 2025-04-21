using Autofac;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using Rebus.Config;

namespace Imget.Orchestrator.Module;

public class OrchestratorModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterHandlers(builder);

        BsonSerializer.RegisterSerializer(new ObjectSerializer(t => ObjectSerializer.DefaultAllowedTypes(t) || t == typeof(ImageSagaData)));
    }

    private static void RegisterHandlers(ContainerBuilder builder)
    {
        builder.RegisterHandlersFromAssemblyOf<OrchestratorModule>();
    }
}