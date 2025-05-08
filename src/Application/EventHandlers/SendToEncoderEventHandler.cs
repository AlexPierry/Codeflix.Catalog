using Application.Interfaces;
using Domain.Events;
using Domain.SeedWork;

namespace Application.EventHandlers;

public class SendToEncoderEventHandler : IDomainEventHandler<VideoUploadedEvent>
{
    private readonly IMessageProducer _messageProducer;

    public SendToEncoderEventHandler(IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
    }

    public async Task HandleAsync(VideoUploadedEvent videoEvent, CancellationToken cancellationToken)
    {
        await _messageProducer.SendMessageAsync(videoEvent, cancellationToken);
    }
}

