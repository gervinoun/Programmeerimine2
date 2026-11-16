using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.InvoiceLines;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    public class InvoiceLinesControllerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_invoice_line_when_id_exists()
        {
            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 5,
                Description = "Rental time",
                Amount = 50.25m
            };
            DbContext.InvoiceLines.Add(invoiceLine);
            await DbContext.SaveChangesAsync();

            using var response = await Client.GetAsync($"/api/InvoiceLines/{invoiceLine.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.True(result.Value.HasValue);
            var value = result.Value.Value;
            Assert.Equal(invoiceLine.Id, value.GetProperty("id").GetInt32());
            Assert.Equal(5, value.GetProperty("invoiceId").GetInt32());
            Assert.Equal("Rental time", value.GetProperty("description").GetString());
            Assert.Equal(50.25m, value.GetProperty("amount").GetDecimal());
        }

        [Fact]
        public async Task Get_should_return_null_value_when_id_is_missing()
        {
            using var response = await Client.GetAsync("/api/InvoiceLines/9999");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task List_should_return_all_invoice_lines_and_filter_by_description()
        {
            DbContext.InvoiceLines.AddRange(
                new InvoiceLine { InvoiceId = 1, Description = "Monthly rental", Amount = 100m },
                new InvoiceLine { InvoiceId = 2, Description = "Cleaning fee", Amount = 25m });
            await DbContext.SaveChangesAsync();

            using var allResponse = await Client.GetAsync("/api/InvoiceLines");
            Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
            var allLines = await allResponse.Content.ReadFromJsonAsync<List<InvoiceLine>>();
            Assert.NotNull(allLines);
            Assert.Equal(2, allLines.Count);

            using var filteredResponse = await Client.GetAsync("/api/InvoiceLines?description=RENTAL");
            Assert.Equal(HttpStatusCode.OK, filteredResponse.StatusCode);
            var filteredLines = await filteredResponse.Content.ReadFromJsonAsync<List<InvoiceLine>>();
            Assert.NotNull(filteredLines);
            Assert.Equal("Monthly rental", Assert.Single(filteredLines).Description);
        }
    }
}
