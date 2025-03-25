using Application.Common;
using Application.Exceptions;
using Application.Interfaces;
using Application.UseCases.Video.Common;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Validation;
using Entities = Domain.Entity;

namespace Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public CreateVideo(
        IVideoRepository videoRepository,
        ICategoryRepository categoryRepository,
        IGenreRepository genreRepository,
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork,
        IStorageService storageService)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<VideoModelOutput> Handle(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var video = new Entities.Video(
            input.Title,
            input.Description,
            input.Opened,
            input.Published,
            input.YearLaunched,
            input.Duration,
            input.MovieRating
        );

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
        {
            throw new EntityValidationException("There are validation errors", validationHandler.Errors());
        }

        await ValidateAndAddRelations(input, video, cancellationToken);

        try
        {
            await UpdateImages(input, video, cancellationToken);

            await UploadVideos(input, video, cancellationToken);

            await _videoRepository.Insert(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return VideoModelOutput.FromVideo(video);

        }
        catch (Exception)
        {
            await ClearStorage(video, cancellationToken);
            throw;
        }
    }

    private async Task UploadVideos(CreateVideoInput input, Entities.Video video, CancellationToken cancellationToken)
    {
        if (input.Media is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.Media.Extension);
            var mediaPath = await _storageService.Upload(fileName, input.Media.FileStream, cancellationToken);
            video.UpdateMedia(mediaPath);
        }

        if (input.Trailer is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.Trailer.Extension);
            var trailerPath = await _storageService.Upload(fileName, input.Trailer.FileStream, cancellationToken);
            video.UpdateTrailer(trailerPath);
        }
    }

    private async Task ClearStorage(Entities.Video video, CancellationToken cancellationToken)
    {
        if (video.Thumb is not null)
            await _storageService.Delete(video.Thumb.Path, cancellationToken);

        if (video.Banner is not null)
            await _storageService.Delete(video.Banner.Path, cancellationToken);

        if (video.ThumbHalf is not null)
            await _storageService.Delete(video.ThumbHalf.Path, cancellationToken);

        if (video.Media is not null)
            await _storageService.Delete(video.Media.FilePath, cancellationToken);

        if (video.Trailer is not null)
            await _storageService.Delete(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UpdateImages(CreateVideoInput input, Entities.Video video, CancellationToken cancellationToken)
    {
        if (input.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.Thumb.Extension);
            var thumbPath = await _storageService.Upload(fileName, input.Thumb.FileStream, cancellationToken);
            video.UpdateThumb(thumbPath);
        }

        if (input.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.Banner.Extension);
            var bannerPath = await _storageService.Upload(fileName, input.Banner.FileStream, cancellationToken);
            video.UpdateBanner(bannerPath);
        }

        if (input.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalf.Extension);
            var thumbHalfPath = await _storageService.Upload(fileName, input.ThumbHalf.FileStream, cancellationToken);
            video.UpdateThumbHalf(thumbHalfPath);
        }
    }

    private async Task ValidateAndAddRelations(CreateVideoInput input, Entities.Video video, CancellationToken cancellationToken)
    {
        if (input.CategoriesIds is not null)
        {
            await ValidateCategoryIds(input, cancellationToken);

            input.CategoriesIds.ToList().ForEach(video.AddCategory);
        }

        if (input.GenresIds is not null)
        {
            await ValidateGenreIds(input, cancellationToken);

            input.GenresIds.ToList().ForEach(video.AddGenre);
        }

        if (input.CastMembersIds is not null)
        {
            await ValidateCastMemberIds(input, cancellationToken);

            input.CastMembersIds.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidateCastMemberIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedCastMembers = await _castMemberRepository.GetIdsListByIds(input.CastMembersIds!.ToList(), cancellationToken);
        if (persistedCastMembers.Count < input.CastMembersIds!.Count)
        {
            throw new RelatedAggregateException(
                $"Related cast member id (or ids) not found: {string.Join(", ",
                    input.CastMembersIds.Except(persistedCastMembers))}");
        }
    }

    private async Task ValidateGenreIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedGenres = await _genreRepository.GetIdsListByIds(input.GenresIds!.ToList(), cancellationToken);
        if (persistedGenres.Count < input.GenresIds!.Count)
        {
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(", ",
                    input.GenresIds.Except(persistedGenres))}");
        }
    }

    private async Task ValidateCategoryIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistedCategories = await _categoryRepository.GetIdsListByIds(input.CategoriesIds!.ToList(), cancellationToken);
        if (persistedCategories.Count < input.CategoriesIds!.Count)
        {
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(", ",
                    input.CategoriesIds.Except(persistedCategories))}");
        }
    }
}
