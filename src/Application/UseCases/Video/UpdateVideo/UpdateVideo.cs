using Application.Exceptions;
using Application.Interfaces;
using Application.UseCases.Video.Common;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Validation;
using Entities = Domain.Entity;

namespace Application.UseCases.Video.UpdateVideo;

public class UpdateVideo : IUpdateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;


    public UpdateVideo(
        IVideoRepository videoRepository,
        IGenreRepository genreRepository,
        ICategoryRepository categoryRepository,
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _videoRepository = videoRepository;
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
        _castMemberRepository = castMemberRepository;
    }

    public async Task<VideoModelOutput> Handle(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var currentVideo = await _videoRepository.Get(input.Id, cancellationToken);
        if (currentVideo is null)
            throw new NotFoundException($"Video with id {input.Id} not found.");

        currentVideo.Update(
            input.Title,
            input.Description,
            input.Opened,
            input.Published,
            input.YearLaunched,
            input.Duration,
            input.MovieRating);

        var validationHandler = new NotificationValidationHandler();
        currentVideo.Validate(validationHandler);
        if (validationHandler.HasErrors())
        {
            throw new EntityValidationException("There are validation errors", validationHandler.Errors());
        }

        await ValidateAndAddRelations(input, currentVideo, cancellationToken);

        await _videoRepository.Update(currentVideo, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return VideoModelOutput.FromVideo(currentVideo);
    }


    private async Task ValidateAndAddRelations(UpdateVideoInput input, Entities.Video video, CancellationToken cancellationToken)
    {

        if (input.CategoryIds is not null)
        {
            await ValidateCategoryIds(input, cancellationToken);
            video.RemoveAllCategories();
            input.CategoryIds.ToList().ForEach(video.AddCategory);
        }


        if (input.GenreIds is not null)
        {
            await ValidateGenreIds(input, cancellationToken);
            video.RemoveAllGenres();
            input.GenreIds.ToList().ForEach(video.AddGenre);
        }


        if (input.CastMemberIds is not null)
        {
            await ValidateCastMemberIds(input, cancellationToken);
            video.RemoveAllCastMembers();
            input.CastMemberIds.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidateGenreIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedGenres = await _genreRepository.GetIdsListByIds(input.GenreIds!.ToList(), cancellationToken);
        if (persistedGenres.Count < input.GenreIds!.Count)
        {
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(", ",
                    input.GenreIds.Except(persistedGenres))}");
        }
    }

    private async Task ValidateCategoryIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedCategories = await _categoryRepository.GetIdsListByIds(input.CategoryIds!.ToList(), cancellationToken);
        if (persistedCategories.Count < input.CategoryIds!.Count)
        {
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(", ",
                    input.CategoryIds.Except(persistedCategories))}");
        }
    }

    private async Task ValidateCastMemberIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedCastMembers = await _castMemberRepository.GetIdsListByIds(input.CastMemberIds!.ToList(), cancellationToken);
        if (persistedCastMembers.Count < input.CastMemberIds!.Count)
        {
            throw new RelatedAggregateException(
                $"Related cast member id (or ids) not found: {string.Join(", ",
                    input.CastMemberIds.Except(persistedCastMembers))}");
        }
    }
}
