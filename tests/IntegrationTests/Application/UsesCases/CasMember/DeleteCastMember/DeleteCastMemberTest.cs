using Application.Exceptions;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using UseCase = Application.UseCases.CastMember;

namespace IntegrationTest.Application.UseCases.CastMember;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberTestFixture _fixture;

    public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCastMemberOk))]
    [Trait("Integration/Application", "CastMember - Use Cases")]
    public async void DeleteCastMemberOk()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var exampleCastMember = _fixture.GetExampleCastMember();
        var tracking = await dbContext.AddAsync(exampleCastMember);
        var exampleList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleList);
        dbContext.SaveChanges();

        tracking.State = EntityState.Detached;

        var input = new UseCase.DeleteCastMemberInput(exampleCastMember.Id);
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);

        // When
        await useCase.Handle(input, CancellationToken.None);

        // Then
        var dbContextToTest = _fixture.CreateDbContext(true);
        var deletedCastMember = await dbContextToTest.CastMembers.FindAsync(exampleCastMember.Id);
        deletedCastMember.Should().BeNull();
        var allCastMembers = await dbContext.CastMembers.ToListAsync();
        allCastMembers.Should().HaveCount(exampleList.Count);
    }

    [Fact(DisplayName = nameof(DeleteThrowsWhenCastMemberDoesNotExist))]
    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    public async void DeleteThrowsWhenCastMemberDoesNotExist()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var exampleList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleList);
        dbContext.SaveChanges();

        var input = new UseCase.DeleteCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"CastMember '{input.Id}' not found.");
    }
}