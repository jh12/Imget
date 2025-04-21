namespace Imget.DataAccess.Hubs;

public interface IImageHub
{
    Task Discovered(string url);
}