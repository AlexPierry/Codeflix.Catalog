using Application.Exceptions;
using Application.UseCases.CastMember;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Entities = Domain.Entity;

namespace UnitTests.Application.CastMember;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateOk))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    [MemberData(
        nameof(UpdateCastMemberTestDataGenerator.GetCastMembersToUpdate),
        parameters: 2,
        MemberType = typeof(UpdateCastMemberTestDataGenerator)
    )]
    public async void UpdateOk(Entities.CastMember exampleCastMember, UpdateCastMemberInput input)
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UpdateCastMember(repositoryMock.Object, unitOfWork.Object);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
        repositoryMock.Verify(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(exampleCastMember, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowExceptionWhenCastMemberNotFound))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async void ThrowExceptionWhenCastMemberNotFound()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"CastMember '{exampleGuid}' not found"));

        var useCase = new UpdateCastMember(repositoryMock.Object, unitOfWork.Object);

        // When
        var task = async () => await useCase.Handle(_fixture.GetValidInput(exampleGuid), CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(exampleGuid, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Entities.CastMember>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(ThrowWhenInvalidName))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async void ThrowWhenInvalidName()
    {
        // Given
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWork = _fixture.GetUnitOfWorkMock();
        var exampleCastMember = _fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);

        var input = _fixture.GetValidInput(exampleCastMember.Id);
        input.Name = null!;
        var useCase = new UpdateCastMember(repositoryMock.Object, unitOfWork.Object);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<EntityValidationException>().WithMessage("Name should not be null or empty.");
        repositoryMock.Verify(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Entities.CastMember>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

}