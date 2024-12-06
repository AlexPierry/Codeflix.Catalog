using Application.UseCases.CastMember;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Entities = Domain.Entity;

namespace UnitTests.Application.CastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async void CreateCastMember()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new CreateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _fixture.GetInput();

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        repositoryMock.Verify(repository => repository.Insert(
            It.Is<Entities.CastMember>(x => x.Name == input.Name && x.Type == input.Type),
            It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBe(default);
    }

    [Theory(DisplayName = nameof(ThrowsWhenNameIsInvalid))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public async void ThrowsWhenNameIsInvalid(string invalidName)
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new CreateCastMember(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _fixture.GetInput();
        input.Name = invalidName;

        // When
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<EntityValidationException>().WithMessage("Name should not be null or empty.");
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Entities.CastMember>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}