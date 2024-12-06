using Application.UseCases.CastMember;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;

namespace IntegrationTest.Application.UseCases.CastMember;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCastMemberOk))]
    [Trait("Integration/Application", "UpdateCastMember - Use Cases")]
    public async Task UpdateCastMemberOk()
    {
        //Given        
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(5);
        var dbContext = _fixture.CreateDbContext();
        await dbContext.CastMembers.AddRangeAsync(exampleCastMembersList);        
        var exampleCastMember = _fixture.GetExampleCastMember();        
        var trackingInfo = await dbContext.AddAsync(exampleCastMember);        
        await dbContext.SaveChangesAsync();
        trackingInfo.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        var repository = new CastMemberRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new UpdateCastMember(repository, unitOfWork);
        exampleCastMember.Update(_fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        var input = _fixture.GetInput(exampleCastMember);

        //When
        var output = await useCase.Handle(input, CancellationToken.None);

        //Then
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCastMember.Id);
        output.Name.Should().Be(exampleCastMember.Name);
        output.Type.Should().Be(exampleCastMember.Type);
        output.CreatedAt.Should().BeSameDateAs(exampleCastMember.CreatedAt);

        var dbCastMember = await _fixture.CreateDbContext(true).CastMembers.FindAsync(exampleCastMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().BeSameDateAs(exampleCastMember.CreatedAt);
    }
}