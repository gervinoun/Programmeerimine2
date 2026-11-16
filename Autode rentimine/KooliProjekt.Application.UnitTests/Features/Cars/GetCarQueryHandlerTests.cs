using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Cars;

namespace KooliProjekt.Application.UnitTests.Features.Cars
{
    public class GetCarQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_projection_for_existing_car_and_null_for_missing_id()
        {
            var car = new Car
            {
                NumberPlate = "ABC-123",
                TypeId = 2,
                Kmrate = 1.25m,
                TimeRate = 10m,
                IsAvailable = true
            };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            var handler = new GetCarQueryHandler(new CarRepository(DbContext));
            var result = await handler.Handle(
                new GetCarQuery { Id = car.Id },
                CancellationToken.None);

            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Equal(car.Id, ReadProperty<int>(result.Value!, nameof(Car.Id)));
            Assert.Equal(car.NumberPlate, ReadProperty<string>(result.Value!, nameof(Car.NumberPlate)));
            Assert.Equal(car.TypeId, ReadProperty<int>(result.Value!, nameof(Car.TypeId)));
            Assert.Equal(car.Kmrate, ReadProperty<decimal>(result.Value!, nameof(Car.Kmrate)));
            Assert.Equal(car.TimeRate, ReadProperty<decimal>(result.Value!, nameof(Car.TimeRate)));
            Assert.Equal(car.IsAvailable, ReadProperty<bool>(result.Value!, nameof(Car.IsAvailable)));

            var missing = await handler.Handle(
                new GetCarQuery { Id = car.Id + 1 },
                CancellationToken.None);

            Assert.False(missing.HasErrors);
            Assert.Null(missing.Value);
        }

        [Fact]
        public async Task Get_should_throw_ArgumentNullException_when_request_is_null()
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var repository = new CarRepository(dbContext);
            var handler = new GetCarQueryHandler(repository);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Get_should_return_null_when_request_id_is_zero_or_less(int id)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var repository = new CarRepository(dbContext);

            var query = new GetCarQuery
            {
                Id = id
            };

            var handler = new GetCarQueryHandler(repository);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        private static T ReadProperty<T>(object value, string propertyName)
        {
            return (T)value.GetType().GetProperty(propertyName)!.GetValue(value)!;
        }
    }
}