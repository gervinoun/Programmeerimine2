using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Bookings;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    public class BookingsControllerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_booking_when_id_exists()
        {
            var booking = new Booking
            {
                UserId = 3,
                CarId = 4,
                StartTime = new System.DateTime(2025, 1, 1),
                EndTime = new System.DateTime(2025, 1, 2),
                KmStart = 100,
                KmEnd = 200,
                Status = "Active"
            };
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();

            using var response = await Client.GetAsync($"/api/Bookings/{booking.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.True(result.Value.HasValue);
            var value = result.Value.Value;
            Assert.Equal(booking.Id, value.GetProperty("id").GetInt32());
            Assert.Equal(booking.UserId, value.GetProperty("userId").GetInt32());
            Assert.Equal(booking.CarId, value.GetProperty("carId").GetInt32());
            Assert.Equal("Active", value.GetProperty("status").GetString());
        }

        [Fact]
        public async Task Get_should_return_null_value_when_id_is_missing()
        {
            using var response = await Client.GetAsync("/api/Bookings/9999");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task List_should_return_all_bookings_and_filter_by_status()
        {
            DbContext.Bookings.AddRange(
                new Booking
                {
                    UserId = 1,
                    CarId = 1,
                    StartTime = new System.DateTime(2025, 1, 1),
                    EndTime = new System.DateTime(2025, 1, 2),
                    KmStart = 100,
                    KmEnd = 200,
                    Status = "Active"
                },
                new Booking
                {
                    UserId = 2,
                    CarId = 2,
                    StartTime = new System.DateTime(2025, 2, 1),
                    EndTime = new System.DateTime(2025, 2, 2),
                    KmStart = 300,
                    KmEnd = 400,
                    Status = "Completed"
                });
            await DbContext.SaveChangesAsync();

            using var allResponse = await Client.GetAsync("/api/Bookings");
            Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
            var allBookings = await allResponse.Content.ReadFromJsonAsync<List<Booking>>();
            Assert.NotNull(allBookings);
            Assert.Equal(2, allBookings.Count);

            using var filteredResponse = await Client.GetAsync("/api/Bookings?status=act");
            Assert.Equal(HttpStatusCode.OK, filteredResponse.StatusCode);
            var filteredBookings = await filteredResponse.Content.ReadFromJsonAsync<List<Booking>>();
            Assert.NotNull(filteredBookings);
            Assert.Equal("Active", Assert.Single(filteredBookings).Status);
        }
    }
}
