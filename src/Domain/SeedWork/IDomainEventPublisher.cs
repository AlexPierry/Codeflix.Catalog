
namespace Domain.SeedWork;

public interface IDomainEventPublisher
{
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : DomainEvent;
}
