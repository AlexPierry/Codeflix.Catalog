namespace Api.Models.Category;

public class UpdateCategoryApiInput
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }

    public UpdateCategoryApiInput(string name, string? description = null, bool? isActive = null)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }
}