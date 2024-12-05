using Domain.Enum;
using Domain.SeedWork;

namespace Domain.Entity;

public class CastMember : AggregateRoot
{
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public CastMemberType Type { get; private set; }

    public CastMember(string name, CastMemberType type) : base()
    {
        Name = name;
        Type = type;
        CreatedAt = DateTime.Now;
    }
}