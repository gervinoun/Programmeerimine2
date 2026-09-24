using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Invoices;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Invoices
{
    public class SaveInvoiceCommandHandlerTests : TestBase
    {
        [Fact]
        public async Task Save_should_add_new_invoice()
        {
            var repository = new InvoiceRepository(DbContext);
            var handler = new SaveInvoiceCommandHandler(repository);

            var command = new SaveInvoiceCommand
            {
                Id = 0,
                BookingId = 1,
                InvoiceDate = new DateTime(2026, 2, 5),
                Total = 150.50m
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var invoice = await repository.GetByIdAsync(1);

            Assert.NotNull(invoice);
            Assert.Equal(1, invoice.BookingId);
            Assert.Equal(new DateTime(2026, 2, 5), invoice.InvoiceDate);
            Assert.Equal(150.50m, invoice.Total);
        }

        [Fact]
        public async Task Save_should_update_existing_invoice()
        {
            var repository = new InvoiceRepository(DbContext);

            var invoice = new Invoice
            {
                BookingId = 1,
                InvoiceDate = new DateTime(2026, 1, 1),
                Total = 100m
            };

            await repository.AddAsync(invoice);
            await repository.SaveChangesAsync();

            var handler = new SaveInvoiceCommandHandler(repository);

            var command = new SaveInvoiceCommand
            {
                Id = invoice.Id,
                BookingId = 5,
                InvoiceDate = new DateTime(2026, 2, 10),
                Total = 250.75m
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var savedInvoice =
                await repository.GetByIdAsync(invoice.Id);

            Assert.NotNull(savedInvoice);
            Assert.Equal(5, savedInvoice.BookingId);
            Assert.Equal(new DateTime(2026, 2, 10), savedInvoice.InvoiceDate);
            Assert.Equal(250.75m, savedInvoice.Total);
        }

        [Fact]
        public async Task Save_should_return_error_if_invoice_does_not_exist()
        {
            var repository = new InvoiceRepository(DbContext);
            var handler = new SaveInvoiceCommandHandler(repository);

            var command = new SaveInvoiceCommand
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