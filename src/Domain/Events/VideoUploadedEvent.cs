using Domain.SeedWork;

namespace Domain.Events;

public class VideoUploadedEvent : DomainEvent
{
    public Guid ResourceId { get; set; }
    public string FilePath { get; set; }

    public VideoUploadedEvent(Guid resourceId, string filePath) : base()
    {
        ResourceId = resourceId;
        FilePath = filePath;
    }
}
