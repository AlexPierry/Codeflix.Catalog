using Domain.SeedWork;

namespace Domain.ValueObjects;

public class Image : ValueObject
{
    public string Path { get; }

    public Image(string path)
    {
        Path = path;
    }

    public override bool Equals(ValueObject? other)
    {
        return other is Image image && Path == image.Path;
    }

    protected override int GetCustomHashCode()
    {
        return HashCode.Combine(Path);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Path;
    }
}