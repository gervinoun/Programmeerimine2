using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Invoices;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    public class InvoicesControllerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_invoice_when_id_exists()
        {
            var invoice = new Invoice
            {
                BookingId = 7,
                InvoiceDate = new System.DateTime(2025, 1, 15),
                Total = 125.50m
            };
            DbContext.Invoices.Add(invoice);
            await DbContext.SaveChangesAsync();

            using var response = await Client.GetAsync($"/api/Invoices/{invoice.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.True(result.Value.HasValue);
            var value = result.Value.Value;
            Assert.Equal(invoice.Id, value.GetProperty("id").GetInt32());
            Assert.Equal(7, value.GetProperty("bookingId").GetInt32());
            Assert.Equal(125.50m, value.GetProperty("total").GetDecimal());
        }

        [Fact]
        public async Task Get_should_return_null_value_when_id_is_missing()
        {
            using var response = await Client.GetAsync("/api/Invoices/9999");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task List_should_return_all_invoices_and_filter_by_invoice_fields()
        {
            DbContext.Invoices.AddRange(
                new Invoice { BookingId = 7, InvoiceDate = new System.DateTime(2025, 1, 15), Total = 125.50m },
                new Invoice { BookingId = 8, InvoiceDate = new System.DateTime(2025, 2, 15), Total = 250m });
            await DbContext.SaveChangesAsync();

            using var allResponse = await Client.GetAsync("/api/Invoices");
            Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
            var allInvoices = await allResponse.Content.ReadFromJsonAsync<List<Invoice>>();
            Assert.NotNull(allInvoices);
            Assert.Equal(2, allInvoices.Count);

            using var bookingResponse = await Client.GetAsync("/api/Invoices?bookingId=7");
            Assert.Equal(HttpStatusCode.OK, bookingResponse.StatusCode);
            var byBooking = await bookingResponse.Content.ReadFromJsonAsync<List<Invoice>>();
            Assert.NotNull(byBooking);
            Assert.Equal(7, Assert.Single(byBooking).BookingId);

            using var dateResponse = await Client.GetAsync("/api/Invoices?invoiceDate=2025-01-15T00%3A00%3A00");
            Assert.Equal(HttpStatusCode.OK, dateResponse.StatusCode);
            var byDate = await dateResponse.Content.ReadFromJsonAsync<List<Invoice>>();
            Assert.NotNull(byDate);
            Assert.Equal(7, Assert.Single(byDate).BookingId);

            using var totalResponse = await Client.GetAsync("/api/Invoices?total=125.50");
            Assert.Equal(HttpStatusCode.OK, totalResponse.StatusCode);
            var byTotal = await totalResponse.Content.ReadFromJsonAsync<List<Invoice>>();
            Assert.NotNull(byTotal);
            Assert.Equal(7, Assert.Single(byTotal).BookingId);
        }
    }
}
