using Microsoft.EntityFrameworkCore;
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

        [Fact]
        public async Task Save_should_create_a_new_invoice_line()
        {
            var result = await PostSaveAsync(new SaveInvoiceLineCommand
            {
                InvoiceId = 5,
                Description = "Rental time",
                Amount = 50.25m
            });

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.InvoiceLines.CountAsync());
            var savedLine = await DbContext.InvoiceLines.SingleAsync();
            Assert.Equal(5, savedLine.InvoiceId);
            Assert.Equal("Rental time", savedLine.Description);
            Assert.Equal(50.25m, savedLine.Amount);
        }

        [Fact]
        public async Task Save_should_update_an_existing_invoice_line()
        {
            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 1,
                Description = "Old description",
                Amount = 10m
            };
            DbContext.InvoiceLines.Add(invoiceLine);
            await DbContext.SaveChangesAsync();

            var result = await PostSaveAsync(new SaveInvoiceLineCommand
            {
                Id = invoiceLine.Id,
                InvoiceId = 5,
                Description = "Updated rental",
                Amount = 125.50m
            });

            Assert.False(result.HasErrors);
            await DbContext.Entry(invoiceLine).ReloadAsync();
            Assert.Equal(1, await DbContext.InvoiceLines.CountAsync());
            Assert.Equal(5, invoiceLine.InvoiceId);
            Assert.Equal("Updated rental", invoiceLine.Description);
            Assert.Equal(125.50m, invoiceLine.Amount);
        }

        [Fact]
        public async Task Save_should_return_error_for_a_missing_invoice_line_id()
        {
            var result = await PostSaveAsync(new SaveInvoiceLineCommand
            {
                Id = 9999,
                InvoiceId = 5,
                Description = "Missing line",
                Amount = 50.25m
            });

            Assert.True(result.HasErrors);
            Assert.Contains("Invoice line not found", result.Errors);
            Assert.Empty(await DbContext.InvoiceLines.ToListAsync());
        }

        [Fact]
        public async Task Save_should_return_validation_error_for_an_invalid_invoice_id()
        {
            var result = await PostSaveAsync(new SaveInvoiceLineCommand
            {
                InvoiceId = 0,
                Description = "Rental time",
                Amount = 50.25m
            });

            Assert.True(result.HasErrors);
            Assert.Equal("Invoice id must be greater than zero", result.PropertyErrors[nameof(SaveInvoiceLineCommand.InvoiceId)]);
            Assert.Empty(await DbContext.InvoiceLines.ToListAsync());
        }

        [Fact]
        public async Task Delete_should_remove_an_existing_invoice_line()
        {
            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 5,
                Description = "Delete line",
                Amount = 50.25m
            };
            DbContext.InvoiceLines.Add(invoiceLine);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(invoiceLine.Id);

            Assert.False(result.HasErrors);
            Assert.False(await DbContext.InvoiceLines.AnyAsync(x => x.Id == invoiceLine.Id));
        }

        [Fact]
        public async Task Delete_should_leave_database_unchanged_for_a_missing_invoice_line()
        {
            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 5,
                Description = "Keep line",
                Amount = 50.25m
            };
            DbContext.InvoiceLines.Add(invoiceLine);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(9999);

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.InvoiceLines.CountAsync());
            Assert.True(await DbContext.InvoiceLines.AnyAsync(x => x.Id == invoiceLine.Id));
        }

        private async Task<OperationResultResponse> PostSaveAsync(SaveInvoiceLineCommand command)
        {
            using var response = await Client.PostAsJsonAsync("/api/InvoiceLines", command);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }

        private async Task<OperationResultResponse> DeleteAsync(int id)
        {
            using var response = await Client.DeleteAsync($"/api/InvoiceLines/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }
    }
}
