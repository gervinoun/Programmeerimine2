using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Cars;

namespace KooliProjekt.Application.UnitTests.Features.Cars
{
    public class ListCarsQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var repository = new CarRepository(dbContext);
            var handler = new ListCarsQueryHandler(repository);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_is_zero_or_less(int page)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var repository = new CarRepository(dbContext);

            var query = new ListCarsQuery
            {
                Page = page,
                PageSize = 10
            };

            var handler = new ListCarsQueryHandler(repository);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_zero_or_less(int pageSize)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var repository = new CarRepository(dbContext);

            var query = new ListCarsQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListCarsQueryHandler(repository);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(101)]
        [InlineData(1000)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_too_large(int pageSize)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var repository = new CarRepository(dbContext);

            var query = new ListCarsQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListCarsQueryHandler(repository);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}