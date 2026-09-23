using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.InvoiceLines;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.UnitTests.Features.InvoiceLines
{
    public class DeleteInvoiceLineCommandHandlerTests : TestBase
    {
        [Fact]
        public void Constructor_should_throw_ArgumentNullException_when_dbContext_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DeleteInvoiceLineCommandHandler(null));
        }

        [Fact]
        public async Task Delete_should_throw_ArgumentNullException_when_request_is_null()
        {
            var handler = new DeleteInvoiceLineCommandHandler(DbContext);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Delete_should_return_when_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var handler = new DeleteInvoiceLineCommandHandler(dbContext);

            var command = new DeleteInvoiceLineCommand { Id = id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_delete_existing_invoice_line()
        {
            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 1,
                Description = "Test line",
                Amount = 50
            };

            await DbContext.InvoiceLines.AddAsync(invoiceLine);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteInvoiceLineCommandHandler(DbContext);
            var command = new DeleteInvoiceLineCommand { Id = invoiceLine.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var deletedInvoiceLine = await DbContext.InvoiceLines
                .FirstOrDefaultAsync(x => x.Id == invoiceLine.Id);

            Assert.Null(deletedInvoiceLine);
        }

        [Fact]
        public async Task Delete_should_do_nothing_when_invoice_line_does_not_exist()
        {
            var handler = new DeleteInvoiceLineCommandHandler(DbContext);
            var command = new DeleteInvoiceLineCommand { Id = 999999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }
    }
}