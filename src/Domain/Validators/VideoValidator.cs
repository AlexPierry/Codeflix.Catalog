using Domain.Entity;
using Domain.Validation;

namespace Domain.Validators;

public class VideoValidator : Validator
{
    private readonly Video _video;
    private const int MaxTitleLength = 255;
    private const int MaxDescriptionLength = 4000;

    public VideoValidator(Video video, ValidationHandler handler) : base(handler)
    {
        _video = video;
    }

    public override void Validate()
    {
        ValidateTitle();

        ValidateDescription();

    }

    private void ValidateTitle()
    {
        if (_video.Title.Length > MaxTitleLength)
        {
            _handler.HandleError($"'{nameof(Video.Title)}' should be less or equal {MaxTitleLength} characters");
        }

        if (string.IsNullOrWhiteSpace(_video.Title))
        {
            _handler.HandleError("'Title' is required");
        }
    }

    private void ValidateDescription()
    {
        if (_video.Description.Length > MaxDescriptionLength)
        {
            _handler.HandleError($"'{nameof(Video.Description)}' should be less or equal {MaxDescriptionLength} characters");
        }

        if (string.IsNullOrWhiteSpace(_video.Description))
        {
            _handler.HandleError("'Description' is required");
        }
    }
}