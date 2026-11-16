using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Users;

namespace KooliProjekt.Application.UnitTests.Features.Users
{
    public class GetUserQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_projection_for_existing_user_and_null_for_missing_id()
        {
            var user = new User
            {
                Name = "Alice Example",
                Email = "alice@example.com",
                PasswordHash = "hash",
                Phone = "555-0100"
            };
            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var handler = new GetUserQueryHandler(new UserRepository(DbContext));
            var result = await handler.Handle(
                new GetUserQuery { Id = user.Id },
                CancellationToken.None);

            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Equal(user.Id, ReadProperty<int>(result.Value!, nameof(User.Id)));
            Assert.Equal(user.Name, ReadProperty<string>(result.Value!, nameof(User.Name)));
            Assert.Equal(user.Email, ReadProperty<string>(result.Value!, nameof(User.Email)));
            Assert.Equal(user.Phone, ReadProperty<string>(result.Value!, nameof(User.Phone)));

            var missing = await handler.Handle(
                new GetUserQuery { Id = user.Id + 1 },
                CancellationToken.None);

            Assert.False(missing.HasErrors);
            Assert.Null(missing.Value);
        }

        [Fact]
        public async Task Get_should_throw_ArgumentNullException_when_request_is_null()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new UserRepository(dbContext);
            var handler = new GetUserQueryHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Get_should_return_null_when_request_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var repository = new UserRepository(dbContext);
            var query = new GetUserQuery { Id = id };
            var handler = new GetUserQueryHandler(repository);

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