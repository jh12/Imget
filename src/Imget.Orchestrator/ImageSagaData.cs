using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Rebus.Sagas;

namespace Imget.Orchestrator;

public class ImageSagaData : ISagaData
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public int Revision { get; set; }

    public string ImageId { get; set; } = null!;

    public bool GotMetadata { get; set; }
    public bool GotFile { get; set; }
    public bool GotTags { get; set; }
    public bool GotThumbnail { get; set; }
    public bool GotOrganized { get; set; }
}