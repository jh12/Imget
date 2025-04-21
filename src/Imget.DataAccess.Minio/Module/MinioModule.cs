using Autofac;
using Imget.DataAccess.Repositories;

namespace Imget.DataAccess.Minio.Module;

public class MinioModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterRepositories(builder);
    }

    private static void RegisterRepositories(ContainerBuilder builder)
    {
        builder.RegisterType<MinioImageRepository>().As<IImageRepository>().SingleInstance();
    }
}