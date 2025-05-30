namespace Application.Interfaces;

public interface IMessageProducer
{
    Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
