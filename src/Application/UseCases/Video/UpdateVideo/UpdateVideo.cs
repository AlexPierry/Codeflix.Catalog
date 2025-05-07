using Application.Common;
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
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;


    public UpdateVideo(
        IVideoRepository videoRepository,
        IGenreRepository genreRepository,
        ICategoryRepository categoryRepository,
        ICastMemberRepository castMemberRepository,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _videoRepository = videoRepository;
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
        _castMemberRepository = castMemberRepository;
        _storageService = storageService;
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

        await UpdateImages(input, currentVideo, cancellationToken);

        await _videoRepository.Update(currentVideo, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return VideoModelOutput.FromVideo(currentVideo);
    }


    private async Task ValidateAndAddRelations(UpdateVideoInput input, Entities.Video video, CancellationToken cancellationToken)
    {
        if (input.CategoryIds is not null)
        {
            video.RemoveAllCategories();
            if (input.CategoryIds.Any())
            {
                await ValidateCategoryIds(input, cancellationToken);
                input.CategoryIds.ToList().ForEach(video.AddCategory);
            }
        }

        if (input.GenreIds is not null)
        {
            video.RemoveAllGenres();
            if (input.GenreIds.Any())
            {
                await ValidateGenreIds(input, cancellationToken);
                input.GenreIds.ToList().ForEach(video.AddGenre);
            }
        }

        if (input.CastMemberIds is not null)
        {
            video.RemoveAllCastMembers();
            if (input.CastMemberIds.Any())
            {
                await ValidateCastMemberIds(input, cancellationToken);
                input.CastMemberIds.ToList().ForEach(video.AddCastMember);
            }
        }
    }

    private async Task ValidateGenreIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedGenres = await _genreRepository.GetIdsListByIds(input.GenreIds ?? [], cancellationToken);
        if (persistedGenres?.Count < input.GenreIds?.Count)
        {
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(", ",
                    input.GenreIds.Except(persistedGenres))}");
        }
    }

    private async Task ValidateCategoryIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedCategories = await _categoryRepository.GetIdsListByIds(input.CategoryIds ?? [], cancellationToken);
        if (persistedCategories?.Count < input.CategoryIds?.Count)
        {
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(", ",
                    input.CategoryIds.Except(persistedCategories))}");
        }
    }

    private async Task ValidateCastMemberIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedCastMembers = await _castMemberRepository.GetIdsListByIds(input.CastMemberIds ?? [], cancellationToken);
        if (persistedCastMembers?.Count < input.CastMemberIds?.Count)
        {
            throw new RelatedAggregateException(
                $"Related cast member id (or ids) not found: {string.Join(", ",
                    input.CastMemberIds.Except(persistedCastMembers))}");
        }
    }

    private async Task UpdateImages(UpdateVideoInput input, Entities.Video video, CancellationToken cancellationToken)
    {
        if (input.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.Thumb.Extension);
            var thumbPath = await _storageService.Upload(fileName, input.Thumb.FileStream, input.Thumb.ContentType, cancellationToken);
            video.UpdateThumb(thumbPath);
        }

        if (input.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.Banner.Extension);
            var bannerPath = await _storageService.Upload(fileName, input.Banner.FileStream, input.Banner.ContentType, cancellationToken);
            video.UpdateBanner(bannerPath);
        }

        if (input.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalf.Extension);
            var thumbHalfPath = await _storageService.Upload(fileName, input.ThumbHalf.FileStream, input.ThumbHalf.ContentType, cancellationToken);
            video.UpdateThumbHalf(thumbHalfPath);
        }
    }
}
