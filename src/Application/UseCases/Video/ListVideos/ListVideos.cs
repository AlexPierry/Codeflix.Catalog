using Domain.Repository;
using Entities = Domain.Entity;

namespace Application.UseCases.Video.ListVideos;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICastMemberRepository _castMemberRepository;

    public ListVideos(IVideoRepository videoRepository, ICategoryRepository categoryRepository,
        IGenreRepository genreRepository, ICastMemberRepository castMemberRepository)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
        _castMemberRepository = castMemberRepository;
    }

    public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
    {
        var searchOutput = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);

        var relatedCategoriesIds = searchOutput.Items.SelectMany(video => video.Categories).Distinct().ToList();
        IReadOnlyList<Entities.Category>? relatedCategories = null;
        if (relatedCategoriesIds.Count > 0)
        {
            relatedCategories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
        }

        var relatedGenresIds = searchOutput.Items.SelectMany(video => video.Genres).Distinct().ToList();
        IReadOnlyList<Entities.Genre>? relatedGenres = null;
        if (relatedGenresIds.Count > 0)
        {
            relatedGenres = await _genreRepository.GetListByIds(relatedGenresIds, cancellationToken);
        }

        var relatedCastMembersIds = searchOutput.Items.SelectMany(video => video.CastMembers).Distinct().ToList();
        IReadOnlyList<Entities.CastMember>? relatedCastMembers = null;
        if (relatedCastMembersIds.Count > 0)
        {
            relatedCastMembers = await _castMemberRepository.GetListByIds(relatedCastMembersIds, cancellationToken);
        }

        return ListVideosOutput.FromSearchOutput(searchOutput, relatedCategories, relatedGenres, relatedCastMembers);
    }
}
