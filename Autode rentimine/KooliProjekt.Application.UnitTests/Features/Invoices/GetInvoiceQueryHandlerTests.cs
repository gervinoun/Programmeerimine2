using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Invoices;

namespace KooliProjekt.Application.UnitTests.Features.Invoices
{
    public class GetInvoiceQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_projection_for_existing_invoice_and_null_for_missing_id()
        {
            var invoice = new Invoice
            {
                BookingId = 5,
                InvoiceDate = new DateTime(2025, 1, 15),
                Total = 125m
            };
            DbContext.Invoices.Add(invoice);
            await DbContext.SaveChangesAsync();

            var handler = new GetInvoiceQueryHandler(new InvoiceRepository(DbContext));
            var result = await handler.Handle(
                new GetInvoiceQuery { Id = invoice.Id },
                CancellationToken.None);

            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Equal(invoice.Id, ReadProperty<int>(result.Value!, nameof(Invoice.Id)));
            Assert.Equal(invoice.BookingId, ReadProperty<int>(result.Value!, nameof(Invoice.BookingId)));
            Assert.Equal(invoice.InvoiceDate, ReadProperty<DateTime>(result.Value!, nameof(Invoice.InvoiceDate)));
            Assert.Equal(invoice.Total, ReadProperty<decimal>(result.Value!, nameof(Invoice.Total)));

            var missing = await handler.Handle(
                new GetInvoiceQuery { Id = invoice.Id + 1 },
                CancellationToken.None);

            Assert.False(missing.HasErrors);
            Assert.Null(missing.Value);
        }

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

        private static T ReadProperty<T>(object value, string propertyName)
        {
            return (T)value.GetType().GetProperty(propertyName)!.GetValue(value)!;
        }
    }
}