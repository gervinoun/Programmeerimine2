using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Bookings;

namespace KooliProjekt.Application.UnitTests.Features.Bookings
{
    public class GetBookingQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_projection_for_existing_booking_and_null_for_missing_id()
        {
            var booking = new Booking
            {
                UserId = 7,
                CarId = 8,
                StartTime = new DateTime(2025, 1, 1),
                EndTime = new DateTime(2025, 1, 2),
                KmStart = 100,
                KmEnd = 200,
                Status = "Confirmed"
            };
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();

            var handler = new GetBookingQueryHandler(new BookingRepository(DbContext));
            var result = await handler.Handle(
                new GetBookingQuery { Id = booking.Id },
                CancellationToken.None);

            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Equal(booking.Id, ReadProperty<int>(result.Value!, nameof(Booking.Id)));
            Assert.Equal(booking.UserId, ReadProperty<int>(result.Value!, nameof(Booking.UserId)));
            Assert.Equal(booking.CarId, ReadProperty<int>(result.Value!, nameof(Booking.CarId)));
            Assert.Equal(booking.StartTime, ReadProperty<DateTime>(result.Value!, nameof(Booking.StartTime)));
            Assert.Equal(booking.EndTime, ReadProperty<DateTime>(result.Value!, nameof(Booking.EndTime)));
            Assert.Equal(booking.KmStart, ReadProperty<decimal>(result.Value!, nameof(Booking.KmStart)));
            Assert.Equal(booking.KmEnd, ReadProperty<decimal>(result.Value!, nameof(Booking.KmEnd)));
            Assert.Equal(booking.Status, ReadProperty<string>(result.Value!, nameof(Booking.Status)));

            var missing = await handler.Handle(
                new GetBookingQuery { Id = booking.Id + 1 },
                CancellationToken.None);

            Assert.False(missing.HasErrors);
            Assert.Null(missing.Value);
        }

        [Fact]
        public async Task Get_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new BookingRepository(dbContext);
            var handler = new GetBookingQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Get_should_return_null_when_request_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new BookingRepository(dbContext);
            var query = new GetBookingQuery { Id = id };
            var handler = new GetBookingQueryHandler(repository);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        private static T ReadProperty<T>(object value, string propertyName)
        {
            return (T)value.GetType().GetProperty(propertyName)!.GetValue(value)!;
        }
    }
}