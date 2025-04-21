using Imget.Shared.Message.Event;

namespace Imget.DataAccess.Services;

public interface IImageEventService
{
    Task ImageProcessedAsync(ImageProcessed image);
    Task ImageDiscoveredAsync(string id);
}