using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Cars;

namespace KooliProjekt.Application.UnitTests.Features.Cars
{
    public class GetCarQueryHandlerTests : TestBase
    {
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
    }
}