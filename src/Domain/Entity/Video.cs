using Domain.Enum;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.Validation;
using Domain.Validators;

namespace Domain.Entity;

public class Video : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int YearLaunched { get; private set; }
    public int Duration { get; private set; }
    public MovieRating MovieRating { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Video(string title, string description, bool opened, bool published,
        int year, int duration, MovieRating movieRating)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = year;
        Duration = duration;
        MovieRating = movieRating;
        CreatedAt = DateTime.Now;
    }

    public void Update(string title, string description, bool opened, bool published,
        int year, int duration, MovieRating movieRating)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = year;
        Duration = duration;
        MovieRating = movieRating;
    }

    public void Validate(ValidationHandler handler)
    {
        new VideoValidator(this, handler).Validate();
    }
}