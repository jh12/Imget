using Imget.DataAccess.Services;
using Imget.Shared.Message.Event;
using Rebus.Handlers;

namespace Imget.DataAccess.Rmq;

public class ImageProcessedHandler : IHandleMessages<ImageProcessed>
{
    private readonly IImageEventService _eventService;

    public ImageProcessedHandler(IImageEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task Handle(ImageProcessed message)
    {
        await _eventService.ImageProcessedAsync(message);
    }
}