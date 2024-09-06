using Domain.SeedWork;

namespace Domain.Entity;

public class Category : AggregateRoot
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Category(string name, string category, bool isActive = true) : base()
    {
        Name = name;
        Description = category;
        IsActive = isActive;
        CreatedAt = DateTime.Now;

        Validate();
    }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Validate();
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description ?? Description;
        Validate();
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, nameof(Name));

        DomainValidation.MinLength(Name, 3, nameof(Name));

        DomainValidation.MaxLength(Name, 255, nameof(Name));

        DomainValidation.NotNull(Description, nameof(Description));

        DomainValidation.MaxLength(Description, 10_000, nameof(Description));
    }
}