using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.CarTypes;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.UnitTests.Features.CarTypes
{
    public class DeleteCarTypeCommandHandlerTests : TestBase
    {
        [Fact]
        public void Constructor_should_throw_ArgumentNullException_when_dbContext_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DeleteCarTypeCommandHandler(null));
        }

        [Fact]
        public async Task Delete_should_throw_ArgumentNullException_when_request_is_null()
        {
            var handler = new DeleteCarTypeCommandHandler(DbContext);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Delete_should_return_when_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var handler = new DeleteCarTypeCommandHandler(dbContext);

            var command = new DeleteCarTypeCommand { Id = id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_delete_existing_car_type()
        {
            var carType = new CarType
            {
                Name = "Test type"
            };

            await DbContext.CarTypes.AddAsync(carType);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteCarTypeCommandHandler(DbContext);
            var command = new DeleteCarTypeCommand { Id = carType.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var deletedCarType = await DbContext.CarTypes
                .FirstOrDefaultAsync(x => x.Id == carType.Id);

            Assert.Null(deletedCarType);
        }

        [Fact]
        public async Task Delete_should_do_nothing_when_car_type_does_not_exist()
        {
            var handler = new DeleteCarTypeCommandHandler(DbContext);
            var command = new DeleteCarTypeCommand { Id = 999999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }
    }
}