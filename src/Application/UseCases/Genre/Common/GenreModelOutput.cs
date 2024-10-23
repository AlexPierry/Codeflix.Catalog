using DomainEntity = Domain.Entity;

namespace Application.UseCases.Genre.Common;

public class GenreModelOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Guid> Catetories { get; set; }

    public GenreModelOutput(Guid id, string name, bool isActive, DateTime createdAt, List<Guid> categories)
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
            genre.Categories.ToList()
        );
    }
}