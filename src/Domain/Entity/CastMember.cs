using Domain.Enum;
using Domain.SeedWork;
using Domain.Validation;

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

        Validate();
    }

    public void Update(string newName, CastMemberType newType)
    {
        Name = newName;
        Type = newType;

        Validate();
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, nameof(Name));
    }
}