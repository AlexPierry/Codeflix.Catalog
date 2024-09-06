using UseCase = Application.UseCases.Category.UpdateCategory;

namespace UnitTests.Application.UpdateCategory;

public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();

        for (int index = 0; index < times; index++)
        {
            var exampleCategory = fixture.GetValidCategory();
            var input = fixture.GetValidInput(exampleCategory.Id);
            yield return new object[] {
                exampleCategory, input
            };
        }
    }
}