using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Cars;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    public class CarsControllerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_car_when_id_exists()
        {
            var car = new Car
            {
                NumberPlate = "ABC-123",
                TypeId = 2,
                Kmrate = 1.25m,
                TimeRate = 10m,
                IsAvailable = true
            };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            using var response = await Client.GetAsync($"/api/Cars/{car.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.True(result.Value.HasValue);
            var value = result.Value.Value;
            Assert.Equal(car.Id, value.GetProperty("id").GetInt32());
            Assert.Equal("ABC-123", value.GetProperty("numberPlate").GetString());
            Assert.Equal(2, value.GetProperty("typeId").GetInt32());
            Assert.True(value.GetProperty("isAvailable").GetBoolean());
        }

        [Fact]
        public async Task Get_should_return_null_value_when_id_is_missing()
        {
            using var response = await Client.GetAsync("/api/Cars/9999");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task List_should_return_all_cars_and_filter_by_number_plate()
        {
            DbContext.Cars.AddRange(
                new Car { NumberPlate = "ABC-123", TypeId = 1, Kmrate = 1, TimeRate = 1, IsAvailable = true },
                new Car { NumberPlate = "XYZ-789", TypeId = 2, Kmrate = 2, TimeRate = 2, IsAvailable = false });
            await DbContext.SaveChangesAsync();

            using var allResponse = await Client.GetAsync("/api/Cars");
            Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
            var allCars = await allResponse.Content.ReadFromJsonAsync<List<Car>>();
            Assert.NotNull(allCars);
            Assert.Equal(2, allCars.Count);

            using var filteredResponse = await Client.GetAsync("/api/Cars?numberPlate=abc");
            Assert.Equal(HttpStatusCode.OK, filteredResponse.StatusCode);
            var filteredCars = await filteredResponse.Content.ReadFromJsonAsync<List<Car>>();
            Assert.NotNull(filteredCars);
            Assert.Equal("ABC-123", Assert.Single(filteredCars).NumberPlate);
        }
    }
}
