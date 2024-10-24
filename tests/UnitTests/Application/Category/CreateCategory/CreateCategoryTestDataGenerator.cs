namespace UnitTests.Application.Category;

public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 10)
    {
        var fixture = new CreateCategoryTestFixture();

        var invalidInputList = new List<object[]>();

        int totalInvalidTests = 5;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidTests)
            {
                case 0:
                    invalidInputList.Add([fixture.GetInvalidInputNameIsNull(), "Name should not be null or empty."]);
                    break;
                case 1:
                    invalidInputList.Add([fixture.GetInvalidInputShortName(), "Name should not be less than 3 characters long."]);
                    break;
                case 2:
                    invalidInputList.Add([fixture.GetInvalidInputLongName(), "Name should not be greater than 255 characters long."]);
                    break;
                case 3:
                    invalidInputList.Add([fixture.GetInvalidInputDescriptionIsNull(), "Description should not be null."]);
                    break;
                case 4:
                    invalidInputList.Add([fixture.GetInvalidInputLongDescription(), "Description should not be greater than 10000 characters long."]);
                    break;
                default:
                    break;
            }
        }

        return invalidInputList;
    }
}