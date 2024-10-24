namespace UnitTests.Application.Category;

public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();

        for (int index = 0; index < times; index++)
        {
            var exampleCategory = fixture.GetExampleCategory();
            var input = fixture.GetValidInput(exampleCategory.Id);
            yield return new object[] {
                exampleCategory, input
            };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new UpdateCategoryTestFixture();

        var invalidInputList = new List<object[]>();

        int totalInvalidTests = 3;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidTests)
            {
                case 0:
                    invalidInputList.Add([fixture.GetInvalidInputShortName(), "Name should not be less than 3 characters long."]);
                    break;
                case 1:
                    invalidInputList.Add([fixture.GetInvalidInputLongName(), "Name should not be greater than 255 characters long."]);
                    break;
                case 2:
                    invalidInputList.Add([fixture.GetInvalidInputLongDescription(), "Description should not be greater than 10000 characters long."]);
                    break;
                default:
                    break;
            }
        }

        return invalidInputList;
    }
}