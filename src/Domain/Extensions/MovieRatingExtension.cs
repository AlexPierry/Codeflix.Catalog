using Domain.Enum;

namespace Domain.Extensions;

public static class MovieRatingExtension
{
    public static MovieRating ToMovieRating(this string rating)
    {
        return rating switch
        {
            "ER" => MovieRating.ER,
            "L" => MovieRating.L,
            "10" => MovieRating.R10,
            "12" => MovieRating.R12,
            "16" => MovieRating.R16,
            "18" => MovieRating.R18,
            _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, null)
        };
    }

    public static string ToFriendlyString(this MovieRating rating)
    {
        return rating switch
        {
            MovieRating.ER => "ER",
            MovieRating.L => "L",
            MovieRating.R10 => "10",
            MovieRating.R12 => "12",
            MovieRating.R16 => "16",
            MovieRating.R18 => "18",
            _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, null)
        };
    }
}
