using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.UnitTests.Features.Users
{
    public class DeleteUserCommandHandlerTests : TestBase
    {
        [Fact]
        public void Constructor_should_throw_ArgumentNullException_when_dbContext_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DeleteUserCommandHandler(null));
        }

        [Fact]
        public async Task Delete_should_throw_ArgumentNullException_when_request_is_null()
        {
            var handler = new DeleteUserCommandHandler(DbContext);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Delete_should_return_when_id_is_zero_or_less(int id)
        {
            var dbContext = GetFaultyDbContext();
            var handler = new DeleteUserCommandHandler(dbContext);

            var command = new DeleteUserCommand { Id = id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_delete_existing_user()
        {
            var user = new User
            {
                Name = "Test User",
                Email = "test@test.ee",
                PasswordHash = "test",
                Phone = "55555555"
            };

            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteUserCommandHandler(DbContext);
            var command = new DeleteUserCommand { Id = user.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var deletedUser = await DbContext.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id);

            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task Delete_should_do_nothing_when_user_does_not_exist()
        {
            var handler = new DeleteUserCommandHandler(DbContext);
            var command = new DeleteUserCommand { Id = 999999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }
    }
}