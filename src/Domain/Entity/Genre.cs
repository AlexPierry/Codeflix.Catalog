using Domain.SeedWork;
using Domain.Validation;

namespace Domain.Entity;

public class Genre : AggregateRoot
{
    private List<Guid> _categories;

    public string Name { get; set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<Guid> Categories => _categories;

    public Genre(string name, bool isActive = true) : base()
    {
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        _categories = new List<Guid>();
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

    public void Update(string newName)
    {
        Name = newName;
        Validate();
    }

    public void AddCategory(Guid categoryId)
    {
        _categories.Add(categoryId);
        Validate();
    }

    public void RemoveCategory(Guid categoryId)
    {
        _categories.Remove(categoryId);
        Validate();
    }

    public void RemoveAllCategories()
    {
        _categories.Clear();
        Validate();
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, nameof(Name));
    }

}