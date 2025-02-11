using Domain.Enum;
using Domain.Extensions;
using FluentAssertions;

namespace UnitTests.Domain.Extensions;

public class MovieRatingExtensionsTest
{
    [Theory(DisplayName = nameof(StringToMovieRating))]
    [Trait("Domain", "Movie Rating - Extensions")]
    [InlineData("ER", MovieRating.ER)]
    [InlineData("L", MovieRating.L)]
    [InlineData("10", MovieRating.R10)]
    [InlineData("12", MovieRating.R12)]
    [InlineData("16", MovieRating.R16)]
    [InlineData("18", MovieRating.R18)]
    public void StringToMovieRating(string enumString, MovieRating expectedRating)
    {
        enumString.ToMovieRating().Should().Be(expectedRating);
    }

    [Fact(DisplayName = nameof(StringToMovieRatingThrowsException))]
    [Trait("Domain", "Movie Rating - Extensions")]
    public void StringToMovieRatingThrowsException()
    {
        FluentActions.Invoking(() => "Invalid".ToMovieRating())
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory(DisplayName = nameof(MovieRatingToFriendlyString))]
    [Trait("Domain", "Movie Rating - Extensions")]
    [InlineData(MovieRating.ER, "ER")]
    [InlineData(MovieRating.L, "L")]
    [InlineData(MovieRating.R10, "10")]
    [InlineData(MovieRating.R12, "12")]
    [InlineData(MovieRating.R16, "16")]
    [InlineData(MovieRating.R18, "18")]
    public void MovieRatingToFriendlyString(MovieRating rating, string expectedString)
    {
        MovieRatingExtension.ToFriendlyString(rating).Should().Be(expectedString);
    }

    [Fact(DisplayName = nameof(MovieRatingToFriendlyStringThrowsException))]
    [Trait("Domain", "Movie Rating - Extensions")]
    public void MovieRatingToFriendlyStringThrowsException()
    {
        FluentActions.Invoking(() => ((MovieRating)100).ToFriendlyString())
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}