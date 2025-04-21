using Imget.DataAccess.Hubs;
using Imget.DataAccess.Services;
using Imget.Hubs;
using Imget.Shared.Message.Event;
using Microsoft.AspNetCore.SignalR;
using MudBlazor;

namespace Imget.Services;

public class ImageEventService : IImageEventService
{
    private readonly IHubContext<ImageHub, IImageHub> _hubContext;

    public ImageEventService(IHubContext<ImageHub, IImageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // TODO: Old
    public async Task ImageProcessedAsync(ImageProcessed image)
    {
        await _hubContext.Clients.All.Discovered(image.ThumbnailPath.AbsoluteUri);
    }

    // TODO: DTO
    public async Task ImageDiscoveredAsync(string id)
    {
        await _hubContext.Clients.All.Discovered($"/image/{id}");
    }
}