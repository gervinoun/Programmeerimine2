using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.InvoiceLines;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.InvoiceLines
{
    public class SaveInvoiceLineCommandHandlerTests : TestBase
    {
        [Fact]
        public async Task Save_should_add_new_invoice_line()
        {
            var repository = new InvoiceLineRepository(DbContext);
            var handler = new SaveInvoiceLineCommandHandler(repository);

            var command = new SaveInvoiceLineCommand
            {
                Id = 0,
                InvoiceId = 1,
                Description = "Rental time",
                Amount = 50.25m
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var invoiceLine = await repository.GetByIdAsync(1);

            Assert.NotNull(invoiceLine);
            Assert.Equal(1, invoiceLine.InvoiceId);
            Assert.Equal("Rental time", invoiceLine.Description);
            Assert.Equal(50.25m, invoiceLine.Amount);
        }

        [Fact]
        public async Task Save_should_update_existing_invoice_line()
        {
            var repository = new InvoiceLineRepository(DbContext);

            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 1,
                Description = "Old description",
                Amount = 10m
            };

            await repository.AddAsync(invoiceLine);
            await repository.SaveChangesAsync();

            var handler = new SaveInvoiceLineCommandHandler(repository);

            var command = new SaveInvoiceLineCommand
            {
                Id = invoiceLine.Id,
                InvoiceId = 5,
                Description = "Kilometres",
                Amount = 125.50m
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var savedInvoiceLine =
                await repository.GetByIdAsync(invoiceLine.Id);

            Assert.NotNull(savedInvoiceLine);
            Assert.Equal(5, savedInvoiceLine.InvoiceId);
            Assert.Equal("Kilometres", savedInvoiceLine.Description);
            Assert.Equal(125.50m, savedInvoiceLine.Amount);
        }

        [Fact]
        public async Task Save_should_return_error_if_invoice_line_does_not_exist()
        {
            var repository = new InvoiceLineRepository(DbContext);
            var handler = new SaveInvoiceLineCommandHandler(repository);

            var command = new SaveInvoiceLineCommand
            {
                Id = 999
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.HasErrors);
            Assert.NotNull(result.Errors);
        }
    }
}