using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.InvoiceLines;

namespace KooliProjekt.Application.UnitTests.Features.InvoiceLines
{
    public class ListInvoiceLinesQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_filter_by_description_case_insensitively()
        {
            await SeedInvoiceLines();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListInvoiceLinesQuery { Description = "RENTAL" },
                CancellationToken.None);

            Assert.Equal("Monthly rental", Assert.Single(result).Description);
        }

        [Fact]
        public async Task List_should_return_all_invoice_lines_when_description_is_omitted()
        {
            await SeedInvoiceLines();

            var handler = CreateHandler();
            var result = await handler.Handle(new ListInvoiceLinesQuery(), CancellationToken.None);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task List_should_return_empty_when_description_has_no_match()
        {
            await SeedInvoiceLines();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListInvoiceLinesQuery { Description = "fuel" },
                CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceLineRepository(dbContext);
            var handler = new ListInvoiceLinesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_is_zero_or_less(int page)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceLineRepository(dbContext);

            var query = new ListInvoiceLinesQuery
            {
                Page = page,
                PageSize = 10
            };

            var handler = new ListInvoiceLinesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_zero_or_less(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceLineRepository(dbContext);

            var query = new ListInvoiceLinesQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListInvoiceLinesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(101)]
        [InlineData(1000)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_too_large(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new InvoiceLineRepository(dbContext);

            var query = new ListInvoiceLinesQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListInvoiceLinesQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        private ListInvoiceLinesQueryHandler CreateHandler()
        {
            return new ListInvoiceLinesQueryHandler(new InvoiceLineRepository(DbContext));
        }

        private async Task SeedInvoiceLines()
        {
            DbContext.InvoiceLines.AddRange(
                new InvoiceLine { InvoiceId = 10, Description = "Monthly rental", Amount = 100m },
                new InvoiceLine { InvoiceId = 11, Description = "Cleaning fee", Amount = 25m });
            await DbContext.SaveChangesAsync();
        }
    }
}