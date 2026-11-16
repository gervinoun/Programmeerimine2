using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Invoices;

namespace KooliProjekt.Application.UnitTests.Features.Invoices
{
    public class ListInvoicesQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_filter_by_invoice_fields()
        {
            await SeedInvoices();

            var handler = CreateHandler();
            var byBookingId = await handler.Handle(
                new ListInvoicesQuery { BookingId = 5 },
                CancellationToken.None);
            var byInvoiceDate = await handler.Handle(
                new ListInvoicesQuery { InvoiceDate = new DateTime(2025, 1, 15) },
                CancellationToken.None);
            var byTotal = await handler.Handle(
                new ListInvoicesQuery { Total = 125m },
                CancellationToken.None);

            Assert.Equal(125m, Assert.Single(byBookingId).Total);
            Assert.Equal(125m, Assert.Single(byInvoiceDate).Total);
            Assert.Equal(5, Assert.Single(byTotal).BookingId);
        }

        [Fact]
        public async Task List_should_return_all_invoices_when_filters_are_omitted()
        {
            await SeedInvoices();

            var handler = CreateHandler();
            var result = await handler.Handle(new ListInvoicesQuery(), CancellationToken.None);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task List_should_return_empty_when_invoice_filter_has_no_match()
        {
            await SeedInvoices();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListInvoicesQuery { BookingId = -1 },
                CancellationToken.None);

            Assert.Empty(result);
        }

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

        private ListInvoicesQueryHandler CreateHandler()
        {
            return new ListInvoicesQueryHandler(new InvoiceRepository(DbContext));
        }

        private async Task SeedInvoices()
        {
            DbContext.Invoices.AddRange(
                new Invoice
                {
                    BookingId = 5,
                    InvoiceDate = new DateTime(2025, 1, 15),
                    Total = 125m
                },
                new Invoice
                {
                    BookingId = 6,
                    InvoiceDate = new DateTime(2025, 2, 15),
                    Total = 200m
                });
            await DbContext.SaveChangesAsync();
        }
    }
}