using Application.UseCases.Genre.Common;
using MediatR;

namespace Application.UseCases.Genre.UpdateGenre;

public class UpdateGenreInput : IRequest<GenreModelOutput>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool? IsActive { get; set; }

    public List<Guid>? Categories { get; set; }

    public UpdateGenreInput(Guid id, string name, bool? isActive = null, List<Guid>? categories = null)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        Categories = categories;
    }
}