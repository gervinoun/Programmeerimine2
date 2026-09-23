using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Invoices;

namespace KooliProjekt.Application.UnitTests.Features.Invoices
{
    public class GetInvoiceQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceRepository(dbContext);
            var handler = new GetInvoiceQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Get_should_return_null_when_request_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceRepository(dbContext);
            var query = new GetInvoiceQuery { Id = id };
            var handler = new GetInvoiceQueryHandler(repository);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }
    }
}