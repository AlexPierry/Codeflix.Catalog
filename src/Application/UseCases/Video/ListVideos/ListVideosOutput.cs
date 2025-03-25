using Entities = Domain.Entity;
using Application.Common;
using Application.UseCases.Video.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

namespace Application.UseCases.Video.ListVideos;

public class ListVideosOutput : PaginatedListOutput<VideoModelOutput>
{
    public ListVideosOutput(int page, int perPage, int total, IReadOnlyList<VideoModelOutput> items)
        : base(page, perPage, total, items)
    {
    }

    public static ListVideosOutput FromSearchOutput(SearchOutput<Entities.Video> searchOutput)
    {
        return new ListVideosOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(VideoModelOutput.FromVideo)
                .ToList()
                .AsReadOnly()
        );
    }
}
