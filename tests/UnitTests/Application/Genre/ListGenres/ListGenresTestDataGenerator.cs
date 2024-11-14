using Application.UseCases.Genre.ListGenres;

namespace UnitTests.Application.Genre;

public class ListGenresTestDataGenerator
{
    public static IEnumerable<object[]> GetInputsWithoutAllParameters(int times = 14)
    {
        var fixture = new ListGenresTestFixture();
        var inputExample = fixture.GetExampleInput();

        for (int i = 0; i < times; i++)
        {
            switch (i % 7)
            {
                case 0:
                default:
                    yield return new object[] { new ListGenresInput() };
                    break;
                case 1:
                    yield return new object[] { new ListGenresInput(inputExample.Page) };
                    break;
                case 2:
                    yield return new object[] { new ListGenresInput(inputExample.Page, inputExample.PerPage) };
                    break;
                case 3:
                    yield return new object[] { new ListGenresInput(inputExample.Page, inputExample.PerPage, inputExample.Search) };
                    break;
                case 4:
                    yield return new object[] { new ListGenresInput(inputExample.Page, inputExample.PerPage, inputExample.Search, inputExample.Sort) };
                    break;
                case 5:
                    yield return new object[] { new ListGenresInput(inputExample.Page, inputExample.PerPage, inputExample.Search, inputExample.Sort, inputExample.Dir) };
                    break;
                case 6:
                    yield return new object[] { inputExample };
                    break;
            }
        }
    }
}