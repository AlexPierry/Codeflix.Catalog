using Application.UseCases.Genre.Common;
using MediatR;

namespace Application.UseCases.Genre.CreateGenre;

public class CreateGenreInput : IRequest<GenreModelOutput>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public List<Guid>? Categories { get; set; }

    public CreateGenreInput(string name, bool isActive = true, List<Guid>? categories = null)
    {
        Name = name;
        IsActive = isActive;
        Categories = categories;
    }
}