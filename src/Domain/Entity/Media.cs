using Domain.Enum;

namespace Domain.Entity;

public class Media : SeedWork.Entity
{
    public string FilePath { get; private set; }
    public string? EncodedPath { get; private set; }
    public MediaStatus Status { get; private set; }

    public Media(string filePath) : base()
    {
        FilePath = filePath;
        Status = MediaStatus.Pending;
    }

    public void UpdateAsSentToEncode()
    {
        Status = MediaStatus.Processing;
    }

    public void UpdateAsEncoded(string encodedPath)
    {
        Status = MediaStatus.Completed;
        EncodedPath = encodedPath;
    }
}
