
namespace Domain.SeedWork;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; private set; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }

}
