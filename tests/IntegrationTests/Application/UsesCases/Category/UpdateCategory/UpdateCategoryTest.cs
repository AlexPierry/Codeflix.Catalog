using Application.Exceptions;
using Application.UseCases.Category.UpdateCategory;
using Domain.Exceptions;
using FluentAssertions;
using Infra.Data.EF;
using Infra.Data.EF.Repositories;
using Entities = Domain.Entity;
using UseCase = Application.UseCases.Category.UpdateCategory;

namespace IntegrationTest.Application.UseCases.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateOk))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateOk(Entities.Category exampleCategory, UpdateCategoryInput input)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);

        var useCase = new UseCase.UpdateCategory(repository, unitOfWorkMock);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateCategoryWithoutIsActive(Entities.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        // Given
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);

        var useCase = new UseCase.UpdateCategory(repository, unitOfWorkMock);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateCategoryOnlyName(Entities.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        // Given
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);

        var useCase = new UseCase.UpdateCategory(repository, unitOfWorkMock);

        // When
        var output = await useCase.Handle(input, CancellationToken.None);

        // Then
        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

        var dbCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateThrowsExceptionWhenCategoryNotFound))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    public async void UpdateThrowsExceptionWhenCategoryNotFound()
    {
        // Given
        var input = _fixture.GetValidInput();
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);

        var useCase = new UseCase.UpdateCategory(repository, unitOfWorkMock);

        // When
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{input.Id}' not found.");
    }

    [Theory(DisplayName = nameof(UpdateThrowsWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateThrowsWhenCantInstantiateCategory(UpdateCategoryInput invalidInput, string expectedMessage)
    {
        // Given
        var dbContext = _fixture.CreateDbContext();
        var exampleCategories = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategories);
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);

        var useCase = new UseCase.UpdateCategory(repository, unitOfWorkMock);
        invalidInput.Id = exampleCategories[0].Id;

        // When
        var task = async () => await useCase.Handle(invalidInput, CancellationToken.None);

        // Then
        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(expectedMessage);
    }
}