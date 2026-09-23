using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Invoices;

namespace KooliProjekt.Application.UnitTests.Features.Invoices
{
    public class ListInvoicesQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceRepository(dbContext);
            var handler = new ListInvoicesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_is_zero_or_less(int page)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceRepository(dbContext);

            var query = new ListInvoicesQuery
            {
                Page = page,
                PageSize = 10
            };

            var handler = new ListInvoicesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_zero_or_less(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceRepository(dbContext);

            var query = new ListInvoicesQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListInvoicesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(101)]
        [InlineData(1000)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_too_large(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceRepository(dbContext);

            var query = new ListInvoicesQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListInvoicesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}