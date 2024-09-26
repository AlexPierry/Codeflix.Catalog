namespace EndToEndTests.Api.Category.UpdateCategory;

public class UpdateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateCategoryApiTestFixture();

        var invalidInputList = new List<object[]>();

        int totalInvalidTests = 3;

        for (int index = 0; index < totalInvalidTests; index++)
        {
            var invalidInput = fixture.GetExampleInput();

            switch (index % totalInvalidTests)
            {
                case 0:
                    invalidInput.Name = fixture.GetInvalidNameTooShort();
                    invalidInputList.Add([invalidInput, "Name should not be less than 3 characters long."]);
                    break;
                case 1:
                    invalidInput.Name = fixture.GetInvalidNameTooLong();
                    invalidInputList.Add([invalidInput, "Name should not be greater than 255 characters long."]);
                    break;
                case 2:
                    invalidInput.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputList.Add([invalidInput, "Description should not be greater than 10000 characters long."]);
                    break;
                default:
                    break;
            }
        }

        return invalidInputList;
    }
}