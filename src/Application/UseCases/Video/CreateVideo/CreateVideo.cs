using Application.Exceptions;
using Application.Interfaces;
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

    public async Task<CreateVideoOutput> Handle(CreateVideoInput input, CancellationToken cancellationToken)
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

        await UpdateMediaImages(input, video, cancellationToken);

        await _videoRepository.Insert(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CreateVideoOutput.FromVideo(video);
    }

    private async Task UpdateMediaImages(CreateVideoInput input, Entities.Video video, CancellationToken cancellationToken)
    {
        if (input.Thumb is not null)
        {
            var thumbPath = await _storageService.Upload(
                $"{video.Id}-thumb.{input.Thumb.Extension}", input.Thumb.FileStream, cancellationToken);
            video.UpdateThumb(thumbPath);
        }

        if (input.Banner is not null)
        {
            var bannerPath = await _storageService.Upload(
                $"{video.Id}-banner.{input.Banner.Extension}", input.Banner.FileStream, cancellationToken);
            video.UpdateBanner(bannerPath);
        }

        if (input.ThumbHalf is not null)
        {
            var thumbHalfPath = await _storageService.Upload(
                $"{video.Id}-thumb-half.{input.ThumbHalf.Extension}", input.ThumbHalf.FileStream, cancellationToken);
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
