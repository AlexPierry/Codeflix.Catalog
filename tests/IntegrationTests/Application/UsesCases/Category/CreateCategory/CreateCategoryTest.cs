using Application;
using Domain.Exceptions;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UseCase = Application.UseCases.Category.CreateCategory;

namespace IntegrationTest.Application.UseCases.Category;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new UseCase.CreateCategory(repository, unitOfWork);
        var input = _fixture.GetInput();

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default(DateTime));

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithNameOnly))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithNameOnly()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );
        var useCase = new UseCase.CreateCategory(repository, unitOfWork);
        var input = new UseCase.CreateCategoryInput(_fixture.GetInput().Name);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().Be(true);
        output.CreatedAt.Should().NotBe(default(DateTime));

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be("");
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithNameAndDescriptionOnly))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithNameAndDescriptionOnly()
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );

        var useCase = new UseCase.CreateCategory(repository, unitOfWork);
        var exampleCategory = _fixture.GetExampleCategory();
        var input = new UseCase.CreateCategoryInput(exampleCategory.Name, exampleCategory.Description);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(true);
        output.CreatedAt.Should().NotBe(default(DateTime));

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(ThrowsWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 5,
        MemberType = typeof(CreateCategoryTestDataGenerator))]
    public async void ThrowsWhenCantInstantiateCategory(UseCase.CreateCategoryInput input, string exceptionMessage)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(
            dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
        );

        var useCase = new UseCase.CreateCategory(repository, unitOfWork);

        // When
        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);

        var dbCategoriesList = await _fixture.CreateDbContext(true).Categories.AsNoTracking().ToListAsync();
        dbCategoriesList.Should().HaveCount(0);
    }
}
