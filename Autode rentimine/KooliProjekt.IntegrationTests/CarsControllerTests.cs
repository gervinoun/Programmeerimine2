using Microsoft.EntityFrameworkCore;
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

        [Fact]
        public async Task Save_should_create_a_new_car()
        {
            var result = await PostSaveAsync(new SaveCarCommand
            {
                NumberPlate = "NEW-123",
                TypeId = 1,
                Kmrate = 1.5m,
                TimeRate = 10m,
                IsAvailable = true
            });

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.Cars.CountAsync());
            var savedCar = await DbContext.Cars.SingleAsync();
            Assert.Equal("NEW-123", savedCar.NumberPlate);
            Assert.Equal(1, savedCar.TypeId);
            Assert.Equal(1.5m, savedCar.Kmrate);
            Assert.Equal(10m, savedCar.TimeRate);
            Assert.True(savedCar.IsAvailable);
        }

        [Fact]
        public async Task Save_should_update_an_existing_car()
        {
            var car = new Car
            {
                NumberPlate = "OLD-123",
                TypeId = 1,
                Kmrate = 1m,
                TimeRate = 5m,
                IsAvailable = false
            };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            var result = await PostSaveAsync(new SaveCarCommand
            {
                Id = car.Id,
                NumberPlate = "UPDATED-123",
                TypeId = 2,
                Kmrate = 2.5m,
                TimeRate = 15m,
                IsAvailable = true
            });

            Assert.False(result.HasErrors);
            await DbContext.Entry(car).ReloadAsync();
            Assert.Equal(1, await DbContext.Cars.CountAsync());
            Assert.Equal("UPDATED-123", car.NumberPlate);
            Assert.Equal(2, car.TypeId);
            Assert.Equal(2.5m, car.Kmrate);
            Assert.Equal(15m, car.TimeRate);
            Assert.True(car.IsAvailable);
        }

        [Fact]
        public async Task Save_should_return_error_for_a_missing_car_id()
        {
            var result = await PostSaveAsync(new SaveCarCommand
            {
                Id = 9999,
                NumberPlate = "MISSING-123",
                TypeId = 1,
                Kmrate = 1m,
                TimeRate = 5m,
                IsAvailable = true
            });

            Assert.True(result.HasErrors);
            Assert.Contains("Car not found", result.Errors);
            Assert.Empty(await DbContext.Cars.ToListAsync());
        }

        [Fact]
        public async Task Save_should_return_validation_error_for_an_empty_number_plate()
        {
            var result = await PostSaveAsync(new SaveCarCommand
            {
                NumberPlate = "",
                TypeId = 1,
                Kmrate = 1m,
                TimeRate = 5m,
                IsAvailable = true
            });

            Assert.True(result.HasErrors);
            Assert.Equal("Number plate is required", result.PropertyErrors[nameof(SaveCarCommand.NumberPlate)]);
            Assert.Empty(await DbContext.Cars.ToListAsync());
        }

        [Fact]
        public async Task Delete_should_remove_an_existing_car()
        {
            var car = new Car
            {
                NumberPlate = "DELETE-123",
                TypeId = 1,
                Kmrate = 1m,
                TimeRate = 5m,
                IsAvailable = true
            };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(car.Id);

            Assert.False(result.HasErrors);
            Assert.False(await DbContext.Cars.AnyAsync(x => x.Id == car.Id));
        }

        [Fact]
        public async Task Delete_should_leave_database_unchanged_for_a_missing_car()
        {
            var car = new Car
            {
                NumberPlate = "KEEP-123",
                TypeId = 1,
                Kmrate = 1m,
                TimeRate = 5m,
                IsAvailable = true
            };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(9999);

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.Cars.CountAsync());
            Assert.True(await DbContext.Cars.AnyAsync(x => x.Id == car.Id));
        }

        private async Task<OperationResultResponse> PostSaveAsync(SaveCarCommand command)
        {
            using var response = await Client.PostAsJsonAsync("/api/Cars", command);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }

        private async Task<OperationResultResponse> DeleteAsync(int id)
        {
            using var response = await Client.DeleteAsync($"/api/Cars/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }
    }
}
