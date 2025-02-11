using FluentAssertions;
using Domain.ValueObjects;

namespace UnitTests.Domain.ValueObjects
{
    public class ImageTest : BaseFixture
    {
        [Fact(DisplayName = nameof(InstantiationOk))]
        [Trait("Domain", "Value Objects - Image")]
        public void InstantiationOk()
        {
            var path = Faker.Image.PicsumUrl();
            var image = new Image(path);
            image.Path.Should().Be(path);
        }

        [Fact(DisplayName = nameof(EqualsByPath))]
        [Trait("Domain", "Value Objects - Image")]
        public void EqualsByPath()
        {
            var path = Faker.Image.PicsumUrl();
            var image1 = new Image(path);
            var image2 = new Image(path);

            var isEqual = image1 == image2;
            isEqual.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(NotEqualsByPath))]
        [Trait("Domain", "Value Objects - Image")]
        public void NotEqualsByPath()
        {
            var path1 = Faker.Image.PicsumUrl();
            var path2 = Faker.Image.PicsumUrl();
            var image1 = new Image(path1);
            var image2 = new Image(path2);

            var isEqual = image1 == image2;
            isEqual.Should().BeFalse();
        }
    }
}
