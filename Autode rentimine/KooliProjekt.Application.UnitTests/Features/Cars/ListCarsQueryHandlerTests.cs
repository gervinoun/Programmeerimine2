using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Cars;

namespace KooliProjekt.Application.UnitTests.Features.Cars
{
    public class ListCarsQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_filter_by_number_plate_case_insensitively()
        {
            await SeedCars();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListCarsQuery { NumberPlate = "abc" },
                CancellationToken.None);

            Assert.Equal("ABC-123", Assert.Single(result).NumberPlate);
        }

        [Fact]
        public async Task List_should_return_all_cars_when_number_plate_is_omitted()
        {
            await SeedCars();

            var handler = CreateHandler();
            var result = await handler.Handle(new ListCarsQuery(), CancellationToken.None);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task List_should_return_empty_when_number_plate_has_no_match()
        {
            await SeedCars();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListCarsQuery { NumberPlate = "missing" },
                CancellationToken.None);

            Assert.Empty(result);
        }

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

        private ListCarsQueryHandler CreateHandler()
        {
            return new ListCarsQueryHandler(new CarRepository(DbContext));
        }

        private async Task SeedCars()
        {
            DbContext.Cars.AddRange(
                new Car
                {
                    NumberPlate = "ABC-123",
                    TypeId = 1,
                    Kmrate = 1,
                    TimeRate = 1,
                    IsAvailable = true
                },
                new Car
                {
                    NumberPlate = "XYZ-789",
                    TypeId = 2,
                    Kmrate = 1,
                    TimeRate = 1,
                    IsAvailable = false
                });
            await DbContext.SaveChangesAsync();
        }
    }
}