using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.CarTypes;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    public class CarTypesControllerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_car_type_when_id_exists()
        {
            var carType = new CarType { Name = "SUV" };
            DbContext.CarTypes.Add(carType);
            await DbContext.SaveChangesAsync();

            using var response = await Client.GetAsync($"/api/CarTypes/{carType.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.True(result.Value.HasValue);
            var value = result.Value.Value;
            Assert.Equal(carType.Id, value.GetProperty("id").GetInt32());
            Assert.Equal("SUV", value.GetProperty("name").GetString());
        }

        [Fact]
        public async Task Get_should_return_null_value_when_id_is_missing()
        {
            using var response = await Client.GetAsync("/api/CarTypes/9999");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task List_should_return_all_car_types_and_filter_by_name()
        {
            DbContext.CarTypes.AddRange(
                new CarType { Name = "SUV" },
                new CarType { Name = "Sedan" });
            await DbContext.SaveChangesAsync();

            using var allResponse = await Client.GetAsync("/api/CarTypes");
            Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
            var allCarTypes = await allResponse.Content.ReadFromJsonAsync<List<CarType>>();
            Assert.NotNull(allCarTypes);
            Assert.Equal(2, allCarTypes.Count);

            using var filteredResponse = await Client.GetAsync("/api/CarTypes?name=suv");
            Assert.Equal(HttpStatusCode.OK, filteredResponse.StatusCode);
            var filteredCarTypes = await filteredResponse.Content.ReadFromJsonAsync<List<CarType>>();
            Assert.NotNull(filteredCarTypes);
            Assert.Equal("SUV", Assert.Single(filteredCarTypes).Name);
        }

        [Fact]
        public async Task Save_should_create_a_new_car_type()
        {
            var result = await PostSaveAsync(new SaveCarTypeCommand { Name = "SUV" });

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.CarTypes.CountAsync());
            Assert.Equal("SUV", (await DbContext.CarTypes.SingleAsync()).Name);
        }

        [Fact]
        public async Task Save_should_update_an_existing_car_type()
        {
            var carType = new CarType { Name = "Old type" };
            DbContext.CarTypes.Add(carType);
            await DbContext.SaveChangesAsync();

            var result = await PostSaveAsync(new SaveCarTypeCommand
            {
                Id = carType.Id,
                Name = "Updated type"
            });

            Assert.False(result.HasErrors);
            await DbContext.Entry(carType).ReloadAsync();
            Assert.Equal(1, await DbContext.CarTypes.CountAsync());
            Assert.Equal("Updated type", carType.Name);
        }

        [Fact]
        public async Task Save_should_return_error_for_a_missing_car_type_id()
        {
            var result = await PostSaveAsync(new SaveCarTypeCommand
            {
                Id = 9999,
                Name = "Missing type"
            });

            Assert.True(result.HasErrors);
            Assert.Contains("Car type not found", result.Errors);
            Assert.Empty(await DbContext.CarTypes.ToListAsync());
        }

        [Fact]
        public async Task Save_should_return_validation_error_for_an_empty_name()
        {
            var result = await PostSaveAsync(new SaveCarTypeCommand { Name = "" });

            Assert.True(result.HasErrors);
            Assert.Equal("Name is required", result.PropertyErrors[nameof(SaveCarTypeCommand.Name)]);
            Assert.Empty(await DbContext.CarTypes.ToListAsync());
        }

        [Fact]
        public async Task Delete_should_remove_an_existing_car_type()
        {
            var carType = new CarType { Name = "Delete type" };
            DbContext.CarTypes.Add(carType);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(carType.Id);

            Assert.False(result.HasErrors);
            Assert.False(await DbContext.CarTypes.AnyAsync(x => x.Id == carType.Id));
        }

        [Fact]
        public async Task Delete_should_leave_database_unchanged_for_a_missing_car_type()
        {
            var carType = new CarType { Name = "Keep type" };
            DbContext.CarTypes.Add(carType);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(9999);

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.CarTypes.CountAsync());
            Assert.True(await DbContext.CarTypes.AnyAsync(x => x.Id == carType.Id));
        }

        private async Task<OperationResultResponse> PostSaveAsync(SaveCarTypeCommand command)
        {
            using var response = await Client.PostAsJsonAsync("/api/CarTypes", command);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }

        private async Task<OperationResultResponse> DeleteAsync(int id)
        {
            using var response = await Client.DeleteAsync($"/api/CarTypes/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }
    }
}
