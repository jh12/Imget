using Autofac;
using Imget.DataAccess.Minio.Module;
using Imget.DataAccess.Minio.Module.Configuration;
using Imget.DataAccess.Rmq.Module;
using Imget.DataAccess.Rmq.Module.Configuration;
using Imget.DataAccess.Services;
using Imget.Orchestrator.Module;
using Imget.Services;
using Serilog;
using ILogger = Serilog.ILogger;
using Logger = Serilog.Core.Logger;

namespace Imget.Module;

public class MainModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterConfiguration(builder);

        ILogger logger = CreateLogger(builder);

        RegisterModules(builder, logger);
        RegisterServices(builder);
    }

    private static void RegisterConfiguration(ContainerBuilder builder)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables("imget")
            .Build();

        RmqConfig rmqConfig = config.GetRequiredSection("Rmq").Get<RmqConfig>()!;
        builder.RegisterInstance(rmqConfig).SingleInstance();

        RebusConfig rebusConfig = config.GetRequiredSection("Rebus").Get<RebusConfig>()!;
        builder.RegisterInstance(rebusConfig).SingleInstance();
        builder.RegisterInstance(rebusConfig.Minio).SingleInstance();
        builder.RegisterInstance(rebusConfig.MongoDb).SingleInstance();

        MinioImageConfig minioImageConfig = config.GetRequiredSection("Minio").Get<MinioImageConfig>()!;
        builder.RegisterInstance(minioImageConfig).SingleInstance();
    }

    private static Logger CreateLogger(ContainerBuilder builder)
    {
        LoggerConfiguration logBuilder = new();

        logBuilder.Enrich.FromLogContext();

        logBuilder.WriteTo.Console();

        Logger logger = logBuilder.CreateLogger();

        Log.Logger = logger;
        builder.RegisterInstance(logger).As<ILogger>().As<IDisposable>().SingleInstance();

        return logger;
    }

    private static void RegisterModules(ContainerBuilder builder, ILogger logger)
    {
        builder.RegisterModule(new MinioModule());
        builder.RegisterModule(new RmqModule(logger));
        builder.RegisterModule(new OrchestratorModule());
    }

    private static void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<ImageEventService>().As<IImageEventService>().SingleInstance();
    }
}