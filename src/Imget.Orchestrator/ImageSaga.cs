using Imget.Shared.Message.Event;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace Imget.Orchestrator;

public class ImageSaga : Saga<ImageSagaData>,
    IAmInitiatedBy<ImageFound>,
    IHandleMessages<ImageMetadataAcquired>,
    IHandleMessages<ImageFileAcquired>,
    IHandleMessages<ImageTagsAcquired>,
    IHandleMessages<ImageThumbnailAcquired>
{
    private readonly IBus _bus;

    public ImageSaga(IBus bus)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    protected override void CorrelateMessages(ICorrelationConfig<ImageSagaData> config)
    {
        config.Correlate<ImageFound>(i => CombineSourceSystemAndImageId(i.SourceSystem, i.ImageId), d => d.ImageId);

        config.Correlate<ImageMetadataAcquired>(i => CombineSourceSystemAndImageId(i.SourceSystem, i.ImageId), d => d.ImageId);
        config.Correlate<ImageFileAcquired>(i => CombineSourceSystemAndImageId(i.SourceSystem, i.ImageId), d => d.ImageId);
        config.Correlate<ImageTagsAcquired>(i => CombineSourceSystemAndImageId(i.SourceSystem, i.ImageId), d => d.ImageId);
        config.Correlate<ImageThumbnailAcquired>(i => CombineSourceSystemAndImageId(i.SourceSystem, i.ImageId), d => d.ImageId);
    }

    private static string CombineSourceSystemAndImageId(string sourceSystem, string imageId)
    {
        return $"{sourceSystem}:{imageId}";
    }

    public async Task Handle(ImageFound message)
    {
        if (!IsNew)
            return;

        Data.ImageId = CombineSourceSystemAndImageId(message.SourceSystem, message.ImageId);

        await PublishToSystem(new AcquireImageMetadata(message.SourceSystem, message.ImageId));
    }

    public async Task Handle(ImageMetadataAcquired message)
    {
        Data.GotMetadata = true;

        await PublishToSystem(new AcquireImageFile(message.SourceSystem, message.ImageId));
    }

    public async Task Handle(ImageFileAcquired message)
    {
        Data.GotFile = true;

        await PublishToSystem(new AcquireImageTags(message.SourceSystem, message.ImageId));
    }

    public async Task Handle(ImageTagsAcquired message)
    {
        Data.GotTags = true;

        await PublishToSystem(new AcquireImageThumbnail(message.SourceSystem, message.ImageId));
    }

    public async Task Handle(ImageThumbnailAcquired message)
    {
        Data.GotThumbnail = true;

        await PublishToSystem(new OrganizeImage(message.SourceSystem, message.ImageId));
    }

    public Task Handle(ImageOrganized _)
    {
        Data.GotOrganized = true;

        MarkAsComplete();

        return Task.CompletedTask;
    }

    private async Task PublishToSystem(ImageEventBase message)
    {
        await _bus.Advanced.Topics.Publish($"Imget.{message.SourceSystem}", message);
    }
}