using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Bookings;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.UnitTests.Features.Bookings
{
    public class DeleteBookingCommandHandlerTests : TestBase
    {
        [Fact]
        public void Constructor_should_throw_ArgumentNullException_when_dbContext_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DeleteBookingCommandHandler(null));
        }

        [Fact]
        public async Task Delete_should_throw_ArgumentNullException_when_request_is_null()
        {
            var handler = new DeleteBookingCommandHandler(DbContext);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Delete_should_return_when_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var handler = new DeleteBookingCommandHandler(dbContext);

            var command = new DeleteBookingCommand { Id = id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_delete_existing_booking()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                KmStart = 100,
                KmEnd = 200,
                Status = "Test"
            };

            await DbContext.Bookings.AddAsync(booking);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteBookingCommandHandler(DbContext);
            var command = new DeleteBookingCommand { Id = booking.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var deletedBooking = await DbContext.Bookings
                .FirstOrDefaultAsync(x => x.Id == booking.Id);

            Assert.Null(deletedBooking);
        }

        [Fact]
        public async Task Delete_should_do_nothing_when_booking_does_not_exist()
        {
            var handler = new DeleteBookingCommandHandler(DbContext);
            var command = new DeleteBookingCommand { Id = 999999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }
    }
}