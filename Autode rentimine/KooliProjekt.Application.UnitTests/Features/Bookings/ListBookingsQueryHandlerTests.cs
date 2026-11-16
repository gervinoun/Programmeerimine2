using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Bookings;

namespace KooliProjekt.Application.UnitTests.Features.Bookings
{
    public class ListBookingsQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_filter_by_status_case_insensitively()
        {
            await SeedBookings();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListBookingsQuery { Status = "act" },
                CancellationToken.None);

            Assert.Equal("Active", Assert.Single(result).Status);
        }

        [Fact]
        public async Task List_should_return_all_bookings_when_status_is_omitted()
        {
            await SeedBookings();

            var handler = CreateHandler();
            var result = await handler.Handle(new ListBookingsQuery(), CancellationToken.None);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task List_should_return_empty_when_status_has_no_match()
        {
            await SeedBookings();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListBookingsQuery { Status = "cancelled" },
                CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new BookingRepository(dbContext);
            var handler = new ListBookingsQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_is_zero_or_less(int page)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new BookingRepository(dbContext);

            var query = new ListBookingsQuery
            {
                Page = page,
                PageSize = 10
            };

            var handler = new ListBookingsQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_zero_or_less(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new BookingRepository(dbContext);

            var query = new ListBookingsQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListBookingsQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(101)]
        [InlineData(1000)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_too_large(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new BookingRepository(dbContext);

            var query = new ListBookingsQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListBookingsQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        private ListBookingsQueryHandler CreateHandler()
        {
            return new ListBookingsQueryHandler(new BookingRepository(DbContext));
        }

        private async Task SeedBookings()
        {
            DbContext.Bookings.AddRange(
                new Booking
                {
                    UserId = 1,
                    CarId = 1,
                    StartTime = new DateTime(2025, 1, 1),
                    EndTime = new DateTime(2025, 1, 2),
                    KmStart = 100,
                    KmEnd = 200,
                    Status = "Active"
                },
                new Booking
                {
                    UserId = 2,
                    CarId = 2,
                    StartTime = new DateTime(2025, 2, 1),
                    EndTime = new DateTime(2025, 2, 2),
                    KmStart = 200,
                    KmEnd = 300,
                    Status = "Completed"
                });
            await DbContext.SaveChangesAsync();
        }
    }
}