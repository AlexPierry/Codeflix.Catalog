using Application.Interfaces;
using Domain.Repository;
using FluentAssertions;
using Moq;
using Entities = Domain.Entity;
using Domain.Exceptions;
using Application.UseCases.Video.CreateVideo;

namespace UnitTests.Application.Video;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _testFixture;

    public CreateVideoTest(CreateVideoTestFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact(DisplayName = nameof(CreateVideoOk))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    public async Task CreateVideoOk()
    {
        // Arrange
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _testFixture.GetValidVideoInput();

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().BeSameDateAs(DateTime.Now);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.MovieRating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);

        repositoryMock.Verify(x => x.Insert(It.Is<Entities.Video>(
            video => video.Title == input.Title &&
            video.Description == input.Description &&
            video.Published == input.Published &&
            video.Duration == input.Duration &&
            video.MovieRating == input.MovieRating &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Uses Cases")]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(CreateVideoInput input, string expectedValidationError)
    {
        // Arrange
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new CreateVideo(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>();
        exceptionAssertion.WithMessage("There are validation errors")
            .Which.Errors!.ToList()[0].Message.Should().Be(expectedValidationError);

        repositoryMock.Verify(x => x.Insert(It.IsAny<Entities.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}
