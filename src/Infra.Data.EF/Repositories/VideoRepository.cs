using Application.Exceptions;
using Domain.Entity;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;
using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.EF.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly CodeflixCatalogDbContext _context;

    private DbSet<Video> _videos => _context.Set<Video>();
    private DbSet<VideosCategories> _videosCategories => _context.Set<VideosCategories>();
    private DbSet<VideosGenres> _videosGenres => _context.Set<VideosGenres>();
    private DbSet<VideosCastMembers> _videosCastMembers => _context.Set<VideosCastMembers>();
    private DbSet<Media> _medias => _context.Set<Media>();

    public VideoRepository(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public Task Delete(Video video, CancellationToken cancellationToken)
    {
        _videosCategories.RemoveRange(_videosCategories
            .Where(x => x.VideoId == video.Id));

        _videosGenres.RemoveRange(_videosGenres
            .Where(x => x.VideoId == video.Id));

        _videosCastMembers.RemoveRange(_videosCastMembers
            .Where(x => x.VideoId == video.Id));

        if (video.Media is not null)
        {
            _medias.Remove(video.Media);
        }

        if (video.Trailer is not null)
        {
            _medias.Remove(video.Trailer);
        }

        _videos.Remove(video);
        return Task.CompletedTask;
    }

    public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
    {
        var video = await _videos.FirstOrDefaultAsync(video => video.Id == id);
        NotFoundException.ThrowIfNull(video, $"Video '{id}' not found");

        var categories = await _videosCategories
            .Where(x => x.VideoId == id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        categories.ForEach(video!.AddCategory);

        var genres = await _videosGenres
            .Where(x => x.VideoId == id)
            .Select(x => x.GenreId)
            .ToListAsync(cancellationToken);
        genres.ForEach(video!.AddGenre);

        var castMembers = await _videosCastMembers
            .Where(x => x.VideoId == id)
            .Select(x => x.CastMemberId)
            .ToListAsync(cancellationToken);
        castMembers.ForEach(video!.AddCastMember);

        return video!;
    }

    public async Task Insert(Video video, CancellationToken cancellationToken)
    {
        await _videos.AddAsync(video);

        if (video.Categories.Count > 0)
        {
            var relations = video.Categories.Select(categoryId => new VideosCategories(categoryId, video.Id));
            await _videosCategories.AddRangeAsync(relations);
        }

        if (video.Genres.Count > 0)
        {
            var relations = video.Genres.Select(genreId => new VideosGenres(genreId, video.Id));
            await _videosGenres.AddRangeAsync(relations);
        }

        if (video.CastMembers.Count > 0)
        {
            var relations = video.CastMembers.Select(castMemberId => new VideosCastMembers(castMemberId, video.Id));
            await _videosCastMembers.AddRangeAsync(relations);
        }
    }

    public Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task Update(Video video, CancellationToken cancellationToken)
    {
        _videos.Update(video);
        if (video.Categories.Count > 0)
        {
            _videosCategories.RemoveRange(_videosCategories
                .Where(x => x.VideoId == video.Id));
            var relations = video.Categories.Select(categoryId => new VideosCategories(categoryId, video.Id));
            await _videosCategories.AddRangeAsync(relations);
        }

        if (video.Genres.Count > 0)
        {
            _videosGenres.RemoveRange(_videosGenres
                .Where(x => x.VideoId == video.Id));
            var relations = video.Genres.Select(genreId => new VideosGenres(genreId, video.Id));
            await _videosGenres.AddRangeAsync(relations);
        }

        if (video.CastMembers.Count > 0)
        {
            _videosCastMembers.RemoveRange(_videosCastMembers
                .Where(x => x.VideoId == video.Id));
            var relations = video.CastMembers.Select(castMemberId => new VideosCastMembers(castMemberId, video.Id));
            await _videosCastMembers.AddRangeAsync(relations);
        }
    }
}
