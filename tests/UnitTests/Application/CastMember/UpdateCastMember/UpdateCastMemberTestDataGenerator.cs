namespace UnitTests.Application.CastMember;

public class UpdateCastMemberTestDataGenerator
{
    public static IEnumerable<object[]> GetCastMembersToUpdate(int times = 10)
    {
        var fixture = new UpdateCastMemberTestFixture();

        for (int index = 0; index < times; index++)
        {
            var exampleCastMember = fixture.GetExampleCastMember();
            var input = fixture.GetValidInput(exampleCastMember.Id);
            yield return new object[] {
                exampleCastMember, input
            };
        }
    }
}