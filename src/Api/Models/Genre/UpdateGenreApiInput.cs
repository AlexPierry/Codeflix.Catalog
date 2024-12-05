namespace Api.Models.Genre;

public class UpdateGenreApiInput
{
    public string Name { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? Categories { get; set; }

    public UpdateGenreApiInput(string name, bool? isActive = null, List<Guid>? categories = null)
    {
        Name = name;
        IsActive = isActive;
        Categories = categories;
    }
}