using DomainEntity = Domain.Entity;

namespace Application.UseCases.Genre.Common;

public class GenreModelOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<GenreModelOutputCategory> Catetories { get; set; }

    public GenreModelOutput()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        IsActive = false;
        CreatedAt = default;
        Catetories = new List<GenreModelOutputCategory>();
    }

    public GenreModelOutput(Guid id, string name, bool isActive, DateTime createdAt,
        IReadOnlyList<GenreModelOutputCategory> categories)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedAt = createdAt;
        Catetories = categories;
    }

    public static GenreModelOutput FromGenre(DomainEntity.Genre genre)
    {
        return new(
            genre.Id,
            genre.Name,
            genre.IsActive,
            genre.CreatedAt,
            genre.Categories.Select(categoryId => new GenreModelOutputCategory(categoryId)).ToList()
        );
    }
}

public class GenreModelOutputCategory
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public GenreModelOutputCategory(Guid id, string? name = null)
    {
        Id = id;
        Name = name;
    }
}