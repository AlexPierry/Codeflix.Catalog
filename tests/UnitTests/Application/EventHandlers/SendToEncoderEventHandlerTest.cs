using Application.EventHandlers;
using Application.Interfaces;
using Domain.Events;
using Moq;

namespace UnitTests.Application.EventHandlers;

public class SendToEncoderEventHandlerTest
{
    [Fact(DisplayName = nameof(HandleAsync))]
    [Trait("Application", "EventHandlers - Unit Tests")]
    public async Task HandleAsync()
    {
        // Arrange
        var messageProducerMock = new Mock<IMessageProducer>();
        messageProducerMock.Setup(x => x.SendMessageAsync(It.IsAny<VideoUploadedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new SendToEncoderEventHandler(messageProducerMock.Object);
        var @event = new VideoUploadedEvent(Guid.NewGuid(), "media/video.mp4");

        // Act
        await handler.HandleAsync(@event, CancellationToken.None);

        // Assert
        messageProducerMock.Verify(x => x.SendMessageAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
    }
}
