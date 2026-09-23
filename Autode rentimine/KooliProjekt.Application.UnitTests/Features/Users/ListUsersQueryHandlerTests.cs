using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Users;

namespace KooliProjekt.Application.UnitTests.Features.Users
{
    public class ListUsersQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new UserRepository(dbContext);
            var handler = new ListUsersQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_is_zero_or_less(int page)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new UserRepository(dbContext);

            var query = new ListUsersQuery
            {
                Page = page,
                PageSize = 10
            };

            var handler = new ListUsersQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_zero_or_less(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new UserRepository(dbContext);

            var query = new ListUsersQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListUsersQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(101)]
        [InlineData(1000)]
        public async Task List_should_throw_ArgumentException_when_page_size_is_too_large(int pageSize)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new UserRepository(dbContext);

            var query = new ListUsersQuery
            {
                Page = 1,
                PageSize = pageSize
            };

            var handler = new ListUsersQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(query, CancellationToken.None));
        }
    }
}