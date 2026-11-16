using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Users;

namespace KooliProjekt.Application.UnitTests.Features.Users
{
    public class ListUsersQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_filter_by_name_and_email_case_insensitively()
        {
            await SeedUsers();

            var handler = CreateHandler();
            var byName = await handler.Handle(
                new ListUsersQuery { Name = "ali" },
                CancellationToken.None);
            var byEmail = await handler.Handle(
                new ListUsersQuery { Email = "EXAMPLE.COM" },
                CancellationToken.None);

            Assert.Equal("Alice Example", Assert.Single(byName).Name);
            Assert.Equal("alice@example.com", Assert.Single(byEmail).Email);
        }

        [Fact]
        public async Task List_should_return_all_users_when_filters_are_omitted()
        {
            await SeedUsers();

            var handler = CreateHandler();
            var result = await handler.Handle(new ListUsersQuery(), CancellationToken.None);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task List_should_return_empty_when_user_filter_has_no_match()
        {
            await SeedUsers();

            var handler = CreateHandler();
            var result = await handler.Handle(
                new ListUsersQuery { Name = "nobody" },
                CancellationToken.None);

            Assert.Empty(result);
        }

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

        private ListUsersQueryHandler CreateHandler()
        {
            return new ListUsersQueryHandler(new UserRepository(DbContext));
        }

        private async Task SeedUsers()
        {
            DbContext.Users.AddRange(
                new User
                {
                    Name = "Alice Example",
                    Email = "alice@example.com",
                    PasswordHash = "hash-1"
                },
                new User
                {
                    Name = "Bob Sample",
                    Email = "bob@sample.com",
                    PasswordHash = "hash-2"
                });
            await DbContext.SaveChangesAsync();
        }
    }
}