using Domain.Enum;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.Validation;
using Domain.Validators;
using Domain.ValueObjects;

namespace Domain.Entity;

public class Video : AggregateRoot
{
    private readonly List<Guid> _categories;
    private readonly List<Guid> _genres;
    private readonly List<Guid> _castMembers;

    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int YearLaunched { get; private set; }
    public int Duration { get; private set; }
    public MovieRating MovieRating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Image? Thumb { get; private set; }
    public Image? ThumbHalf { get; private set; }
    public Image? Banner { get; private set; }
    public Media? Media { get; private set; }
    public Media? Trailer { get; private set; }
    public IReadOnlyList<Guid> Categories => _categories;
    public IReadOnlyList<Guid> Genres => _genres;
    public IReadOnlyList<Guid> CastMembers => _castMembers;

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
        _categories = new List<Guid>();
        _genres = new List<Guid>();
        _castMembers = new List<Guid>();
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

    public void UpdateThumb(string imagePath)
    {
        Thumb = new Image(imagePath);
    }

    public void UpdateThumbHalf(string imagePath)
    {
        ThumbHalf = new Image(imagePath);
    }

    public void UpdateBanner(string imagePath)
    {
        Banner = new Image(imagePath);
    }

    public void UpdateMedia(string mediaPath)
    {
        Media = new Media(mediaPath);
    }

    public void UpdateTrailer(string mediaPath)
    {
        Trailer = new Media(mediaPath);
    }

    public void UpdateAsSentToEncode()
    {
        if (Media is null)
        {
            throw new EntityValidationException("There is no media.");
        }
        Media.UpdateAsSentToEncode();
    }

    public void UpdateAsEncoded(string encodedMediaPath)
    {
        if (Media is null)
        {
            throw new EntityValidationException("There is no media.");
        }
        Media.UpdateAsEncoded(encodedMediaPath);
    }

    public void AddCategory(Guid categoryId)
    {
        _categories.Add(categoryId);
    }

    public void RemoveCategory(Guid categoryId)
    {
        _categories.Remove(categoryId);
    }

    public void RemoveAllCategories()
    {
        _categories.Clear();
    }

    public void AddGenre(Guid genreId)
    {
        _genres.Add(genreId);
    }

    public void RemoveGenre(Guid genreId)
    {
        _genres.Remove(genreId);
    }

    public void RemoveAllGenres()
    {
        _genres.Clear();
    }

    public void AddCastMember(Guid castMemberId)
    {
        _castMembers.Add(castMemberId);
    }

    public void RemoveCastMember(Guid castMemberId)
    {
        _castMembers.Remove(castMemberId);
    }

    public void RemoveAllCastMembers()
    {
        _castMembers.Clear();
    }
}