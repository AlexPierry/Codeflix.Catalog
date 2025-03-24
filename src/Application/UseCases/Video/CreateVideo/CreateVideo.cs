using Application.Exceptions;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Validation;
using MediatR;
using Entities = Domain.Entity;

namespace Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideo(IVideoRepository videoRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
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

        if (input.CategoriesIds != null)
        {
            var persistedCategories = await _categoryRepository.GetIdsListByIds(input.CategoriesIds.ToList(), cancellationToken);
            if (persistedCategories.Count < input.CategoriesIds.Count)
            {
                throw new RelatedAggregateException(
                    $"Related category id (or ids) not found: {string.Join(", ",
                        input.CategoriesIds.Except(persistedCategories))}");
            }

            foreach (var categoryId in input.CategoriesIds)
            {
                video.AddCategory(categoryId);
            }
        }

        await _videoRepository.Insert(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CreateVideoOutput.FromVideo(video);
    }
}
