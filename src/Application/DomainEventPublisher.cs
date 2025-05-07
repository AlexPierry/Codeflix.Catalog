using Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) 
        where TDomainEvent : DomainEvent
    {
        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();
        if (handlers == null || !handlers.Any())
        {
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent, cancellationToken);
        }

    }
}
