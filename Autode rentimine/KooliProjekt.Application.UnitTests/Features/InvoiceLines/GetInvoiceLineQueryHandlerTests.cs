using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.InvoiceLines;

namespace KooliProjekt.Application.UnitTests.Features.InvoiceLines
{
    public class GetInvoiceLineQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceLineRepository(dbContext);
            var handler = new GetInvoiceLineQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Get_should_return_null_when_request_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceLineRepository(dbContext);
            var query = new GetInvoiceLineQuery { Id = id };
            var handler = new GetInvoiceLineQueryHandler(repository);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }
    }
}