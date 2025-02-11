namespace Domain.Validation;

public class NotificationValidationHandler : ValidationHandler
{
    private readonly List<ValidationError> _errors = [];

    public override void HandleError(ValidationError error)
    {
        _errors.Add(error);
    }

    public IReadOnlyCollection<ValidationError> Errors() => _errors.AsReadOnly();

    public bool HasErrors() => _errors.Any();
}