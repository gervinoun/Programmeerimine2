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
    }
}
