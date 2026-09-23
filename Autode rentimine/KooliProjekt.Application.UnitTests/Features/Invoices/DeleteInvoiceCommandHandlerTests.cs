using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Invoices;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.UnitTests.Features.Invoices
{
    public class DeleteInvoiceCommandHandlerTests : TestBase
    {
        [Fact]
        public void Constructor_should_throw_ArgumentNullException_when_dbContext_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DeleteInvoiceCommandHandler(null));
        }

        [Fact]
        public async Task Delete_should_throw_ArgumentNullException_when_request_is_null()
        {
            var handler = new DeleteInvoiceCommandHandler(DbContext);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Delete_should_return_when_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var handler = new DeleteInvoiceCommandHandler(dbContext);

            var command = new DeleteInvoiceCommand { Id = id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_delete_existing_invoice()
        {
            var invoice = new Invoice
            {
                BookingId = 1,
                InvoiceDate = DateTime.Now,
                Total = 100
            };

            await DbContext.Invoices.AddAsync(invoice);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteInvoiceCommandHandler(DbContext);
            var command = new DeleteInvoiceCommand { Id = invoice.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var deletedInvoice = await DbContext.Invoices
                .FirstOrDefaultAsync(x => x.Id == invoice.Id);

            Assert.Null(deletedInvoice);
        }

        [Fact]
        public async Task Delete_should_do_nothing_when_invoice_does_not_exist()
        {
            var handler = new DeleteInvoiceCommandHandler(DbContext);
            var command = new DeleteInvoiceCommand { Id = 999999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }
    }
}