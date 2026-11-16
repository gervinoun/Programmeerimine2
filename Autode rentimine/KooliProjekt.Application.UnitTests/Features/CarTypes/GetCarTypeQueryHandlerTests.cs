using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.CarTypes;

namespace KooliProjekt.Application.UnitTests.Features.CarTypes
{
    public class GetCarTypeQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_projection_for_existing_car_type_and_null_for_missing_id()
        {
            var carType = new CarType { Name = "SUV" };
            DbContext.CarTypes.Add(carType);
            await DbContext.SaveChangesAsync();

            var handler = new GetCarTypeQueryHandler(new CarTypeRepository(DbContext));
            var result = await handler.Handle(
                new GetCarTypeQuery { Id = carType.Id },
                CancellationToken.None);

            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Equal(carType.Id, ReadProperty<int>(result.Value!, nameof(CarType.Id)));
            Assert.Equal(carType.Name, ReadProperty<string>(result.Value!, nameof(CarType.Name)));

            var missing = await handler.Handle(
                new GetCarTypeQuery { Id = carType.Id + 1 },
                CancellationToken.None);

            Assert.False(missing.HasErrors);
            Assert.Null(missing.Value);
        }

        [Fact]
        public async Task Get_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new CarTypeRepository(dbContext);
            var handler = new GetCarTypeQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Get_should_return_null_when_request_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new CarTypeRepository(dbContext);
            var query = new GetCarTypeQuery { Id = id };
            var handler = new GetCarTypeQueryHandler(repository);

            var result = await handler.Handle(query, CancellationToken.None);

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