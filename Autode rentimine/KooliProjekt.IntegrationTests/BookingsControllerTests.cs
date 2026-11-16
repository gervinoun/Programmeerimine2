using Microsoft.EntityFrameworkCore;
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

        [Fact]
        public async Task Save_should_create_a_new_booking()
        {
            var result = await PostSaveAsync(new SaveBookingCommand
            {
                UserId = 1,
                CarId = 1,
                StartTime = new System.DateTime(2025, 3, 1),
                EndTime = new System.DateTime(2025, 3, 2),
                KmStart = 100,
                KmEnd = 200,
                Status = "Active"
            });

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.Bookings.CountAsync());
            var savedBooking = await DbContext.Bookings.SingleAsync();
            Assert.Equal(1, savedBooking.UserId);
            Assert.Equal(1, savedBooking.CarId);
            Assert.Equal(new System.DateTime(2025, 3, 1), savedBooking.StartTime);
            Assert.Equal(new System.DateTime(2025, 3, 2), savedBooking.EndTime);
            Assert.Equal(100, savedBooking.KmStart);
            Assert.Equal(200, savedBooking.KmEnd);
            Assert.Equal("Active", savedBooking.Status);
        }

        [Fact]
        public async Task Save_should_update_an_existing_booking()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 1,
                StartTime = new System.DateTime(2025, 1, 1),
                EndTime = new System.DateTime(2025, 1, 2),
                KmStart = 10,
                KmEnd = 20,
                Status = "Active"
            };
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();

            var result = await PostSaveAsync(new SaveBookingCommand
            {
                Id = booking.Id,
                UserId = 2,
                CarId = 3,
                StartTime = new System.DateTime(2025, 4, 1),
                EndTime = new System.DateTime(2025, 4, 3),
                KmStart = 30,
                KmEnd = 50,
                Status = "Completed"
            });

            Assert.False(result.HasErrors);
            await DbContext.Entry(booking).ReloadAsync();
            Assert.Equal(1, await DbContext.Bookings.CountAsync());
            Assert.Equal(2, booking.UserId);
            Assert.Equal(3, booking.CarId);
            Assert.Equal(new System.DateTime(2025, 4, 1), booking.StartTime);
            Assert.Equal(new System.DateTime(2025, 4, 3), booking.EndTime);
            Assert.Equal(30, booking.KmStart);
            Assert.Equal(50, booking.KmEnd);
            Assert.Equal("Completed", booking.Status);
        }

        [Fact]
        public async Task Save_should_return_error_for_a_missing_booking_id()
        {
            var result = await PostSaveAsync(new SaveBookingCommand
            {
                Id = 9999,
                UserId = 1,
                CarId = 1,
                StartTime = new System.DateTime(2025, 3, 1),
                EndTime = new System.DateTime(2025, 3, 2),
                KmStart = 100,
                KmEnd = 200,
                Status = "Active"
            });

            Assert.True(result.HasErrors);
            Assert.Contains("Booking not found", result.Errors);
            Assert.Empty(await DbContext.Bookings.ToListAsync());
        }

        [Fact]
        public async Task Save_should_return_validation_error_for_an_empty_status()
        {
            var result = await PostSaveAsync(new SaveBookingCommand
            {
                UserId = 1,
                CarId = 1,
                StartTime = new System.DateTime(2025, 3, 1),
                EndTime = new System.DateTime(2025, 3, 2),
                KmStart = 100,
                KmEnd = 200,
                Status = ""
            });

            Assert.True(result.HasErrors);
            Assert.Equal("Status is required", result.PropertyErrors[nameof(SaveBookingCommand.Status)]);
            Assert.Empty(await DbContext.Bookings.ToListAsync());
        }

        [Fact]
        public async Task Delete_should_remove_an_existing_booking()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 1,
                StartTime = new System.DateTime(2025, 1, 1),
                EndTime = new System.DateTime(2025, 1, 2),
                KmStart = 10,
                KmEnd = 20,
                Status = "Active"
            };
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(booking.Id);

            Assert.False(result.HasErrors);
            Assert.False(await DbContext.Bookings.AnyAsync(x => x.Id == booking.Id));
        }

        [Fact]
        public async Task Delete_should_leave_database_unchanged_for_a_missing_booking()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 1,
                StartTime = new System.DateTime(2025, 1, 1),
                EndTime = new System.DateTime(2025, 1, 2),
                KmStart = 10,
                KmEnd = 20,
                Status = "Active"
            };
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(9999);

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.Bookings.CountAsync());
            Assert.True(await DbContext.Bookings.AnyAsync(x => x.Id == booking.Id));
        }

        private async Task<OperationResultResponse> PostSaveAsync(SaveBookingCommand command)
        {
            using var response = await Client.PostAsJsonAsync("/api/Bookings", command);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }

        private async Task<OperationResultResponse> DeleteAsync(int id)
        {
            using var response = await Client.DeleteAsync($"/api/Bookings/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }
    }
}
