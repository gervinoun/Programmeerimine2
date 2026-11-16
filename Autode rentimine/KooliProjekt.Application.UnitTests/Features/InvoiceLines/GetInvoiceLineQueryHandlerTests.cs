using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.InvoiceLines;

namespace KooliProjekt.Application.UnitTests.Features.InvoiceLines
{
    public class GetInvoiceLineQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_projection_for_existing_invoice_line_and_null_for_missing_id()
        {
            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 12,
                Description = "Rental time",
                Amount = 50.25m
            };
            DbContext.InvoiceLines.Add(invoiceLine);
            await DbContext.SaveChangesAsync();

            var handler = new GetInvoiceLineQueryHandler(new InvoiceLineRepository(DbContext));
            var result = await handler.Handle(
                new GetInvoiceLineQuery { Id = invoiceLine.Id },
                CancellationToken.None);

            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Equal(invoiceLine.Id, ReadProperty<int>(result.Value!, nameof(InvoiceLine.Id)));
            Assert.Equal(invoiceLine.InvoiceId, ReadProperty<int>(result.Value!, nameof(InvoiceLine.InvoiceId)));
            Assert.Equal(invoiceLine.Description, ReadProperty<string>(result.Value!, nameof(InvoiceLine.Description)));
            Assert.Equal(invoiceLine.Amount, ReadProperty<decimal>(result.Value!, nameof(InvoiceLine.Amount)));

            var missing = await handler.Handle(
                new GetInvoiceLineQuery { Id = invoiceLine.Id + 1 },
                CancellationToken.None);

            Assert.False(missing.HasErrors);
            Assert.Null(missing.Value);
        }

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

        private static T ReadProperty<T>(object value, string propertyName)
        {
            return (T)value.GetType().GetProperty(propertyName)!.GetValue(value)!;
        }
    }
}