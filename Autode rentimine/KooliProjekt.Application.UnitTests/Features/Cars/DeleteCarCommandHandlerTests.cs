using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Cars;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.UnitTests.Features.Cars
{
    public class DeleteCarCommandHandlerTests : TestBase
    {
        [Fact]
        public void Constructor_should_throw_ArgumentNullException_when_dbContext_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DeleteCarCommandHandler(null));
        }

        [Fact]
        public async Task Delete_should_throw_ArgumentNullException_when_request_is_null()
        {
            var handler = new DeleteCarCommandHandler(DbContext);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Delete_should_return_when_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var handler = new DeleteCarCommandHandler(dbContext);

            var command = new DeleteCarCommand
            {
                Id = id
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_delete_existing_car()
        {
            var car = new Car
            {
                NumberPlate = "TEST123",
                TypeId = 1,
                Kmrate = 1,
                TimeRate = 1,
                IsAvailable = true
            };

            await DbContext.Cars.AddAsync(car);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteCarCommandHandler(DbContext);

            var command = new DeleteCarCommand
            {
                Id = car.Id
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var deletedCar = await DbContext.Cars
                .FirstOrDefaultAsync(x => x.Id == car.Id);

            Assert.Null(deletedCar);
        }

        [Fact]
        public async Task Delete_should_do_nothing_when_car_does_not_exist()
        {
            var handler = new DeleteCarCommandHandler(DbContext);

            var command = new DeleteCarCommand
            {
                Id = 999999
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }
    }
}