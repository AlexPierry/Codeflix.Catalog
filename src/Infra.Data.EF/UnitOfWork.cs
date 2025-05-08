using Application.Interfaces;
using Domain.SeedWork;
using Microsoft.Extensions.Logging;

namespace Infra.Data.EF;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCatalogDbContext _context;
    private readonly IDomainEventPublisher _domainEventPublisher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(CodeflixCatalogDbContext context, IDomainEventPublisher domainEventPublisher, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _domainEventPublisher = domainEventPublisher;
        _logger = logger;
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        _logger.LogInformation("Commit: {AggregateCount} aggregate roots with events", domainEntities.Count);

        var events = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        _logger.LogInformation("Commit: {EventsCount} domain events to publish", events.Count);
        foreach (var @event in events)
        {
            _logger.LogInformation("Commit: Publishing event {EventName}", @event.GetType().Name);
            await _domainEventPublisher.PublishAsync((dynamic)@event, cancellationToken);
        }

        _logger.LogInformation($"Commit: Clearing domain events");
        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}