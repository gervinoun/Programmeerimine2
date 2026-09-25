using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Bookings;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Bookings
{
    public class SaveBookingCommandHandlerTests : TestBase
    {
        [Fact]
        public async Task Save_should_add_new_booking()
        {
            var repository = new BookingRepository(DbContext);
            var handler = new SaveBookingCommandHandler(repository);

            var command = new SaveBookingCommand
            {
                Id = 0,
                UserId = 1,
                CarId = 2,
                StartTime = new DateTime(2026, 2, 5, 10, 0, 0),
                EndTime = new DateTime(2026, 2, 6, 10, 0, 0),
                KmStart = 100,
                KmEnd = 200,
                Status = "Active"
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var booking = await repository.GetByIdAsync(1);

            Assert.NotNull(booking);
            Assert.Equal(1, booking.UserId);
            Assert.Equal(2, booking.CarId);
            Assert.Equal(100, booking.KmStart);
            Assert.Equal(200, booking.KmEnd);
            Assert.Equal("Active", booking.Status);
        }

        [Fact]
        public async Task Save_should_update_existing_booking()
        {
            var repository = new BookingRepository(DbContext);

            var booking = new Booking
            {
                UserId = 1,
                CarId = 1,
                StartTime = new DateTime(2026, 1, 1),
                EndTime = new DateTime(2026, 1, 2),
                KmStart = 10,
                KmEnd = 20,
                Status = "Old"
            };

            await repository.AddAsync(booking);
            await repository.SaveChangesAsync();

            var handler = new SaveBookingCommandHandler(repository);

            var command = new SaveBookingCommand
            {
                Id = booking.Id,
                UserId = 5,
                CarId = 6,
                StartTime = new DateTime(2026, 2, 5),
                EndTime = new DateTime(2026, 2, 10),
                KmStart = 500,
                KmEnd = 800,
                Status = "Completed"
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var savedBooking =
                await repository.GetByIdAsync(booking.Id);

            Assert.NotNull(savedBooking);
            Assert.Equal(5, savedBooking.UserId);
            Assert.Equal(6, savedBooking.CarId);
            Assert.Equal(new DateTime(2026, 2, 5), savedBooking.StartTime);
            Assert.Equal(new DateTime(2026, 2, 10), savedBooking.EndTime);
            Assert.Equal(500, savedBooking.KmStart);
            Assert.Equal(800, savedBooking.KmEnd);
            Assert.Equal("Completed", savedBooking.Status);
        }

        [Fact]
        public async Task Save_should_return_error_if_booking_does_not_exist()
        {
            var repository = new BookingRepository(DbContext);
            var handler = new SaveBookingCommandHandler(repository);

            var command = new SaveBookingCommand
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

        [Fact]
        public void SaveValidator_should_return_error_when_id_is_negative()
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.Id = -1;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "Id");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SaveValidator_should_return_error_when_user_id_is_invalid(int userId)
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.UserId = userId;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "UserId");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SaveValidator_should_return_error_when_car_id_is_invalid(int carId)
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.CarId = carId;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "CarId");
        }

        [Fact]
        public void SaveValidator_should_return_error_when_start_time_is_empty()
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.StartTime = default;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "StartTime");
        }

        [Fact]
        public void SaveValidator_should_return_error_when_end_time_is_empty()
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.EndTime = default;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "EndTime");
        }

        [Fact]
        public void SaveValidator_should_return_error_when_end_time_is_before_start_time()
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.StartTime = new DateTime(2026, 2, 10);
            command.EndTime = new DateTime(2026, 2, 5);

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "EndTime");
        }

        [Fact]
        public void SaveValidator_should_return_error_when_km_start_is_negative()
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.KmStart = -1;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "KmStart");
        }

        [Fact]
        public void SaveValidator_should_return_error_when_km_end_is_negative()
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.KmEnd = -1;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "KmEnd");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_error_when_status_is_invalid(string status)
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();
            command.Status = status;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                x => x.PropertyName == "Status");
        }

        [Fact]
        public void SaveValidator_should_return_true_when_command_is_valid()
        {
            var validator = new SaveBookingCommandValidator();

            var command = CreateValidCommand();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
        }

        private SaveBookingCommand CreateValidCommand()
        {
            return new SaveBookingCommand
            {
                Id = 0,
                UserId = 1,
                CarId = 1,
                StartTime = new DateTime(2026, 2, 5),
                EndTime = new DateTime(2026, 2, 6),
                KmStart = 100,
                KmEnd = 200,
                Status = "Active"
            };
        }
    }
}