using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Users;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Users
{
    public class SaveUserCommandHandlerTests : TestBase
    {
        [Fact]
        public async Task Save_should_add_new_user()
        {
            var repository = new UserRepository(DbContext);
            var handler = new SaveUserCommandHandler(repository);

            var command = new SaveUserCommand
            {
                Id = 0,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "password123",
                Phone = "55512345"
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var user = await repository.GetByIdAsync(1);

            Assert.NotNull(user);
            Assert.Equal("Test User", user.Name);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("password123", user.PasswordHash);
            Assert.Equal("55512345", user.Phone);
        }

        [Fact]
        public async Task Save_should_update_existing_user()
        {
            var repository = new UserRepository(DbContext);

            var user = new User
            {
                Name = "Old Name",
                Email = "old@example.com",
                PasswordHash = "oldpassword",
                Phone = "11111111"
            };

            await repository.AddAsync(user);
            await repository.SaveChangesAsync();

            var handler = new SaveUserCommandHandler(repository);

            var command = new SaveUserCommand
            {
                Id = user.Id,
                Name = "New Name",
                Email = "new@example.com",
                PasswordHash = "newpassword",
                Phone = "99999999"
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var savedUser =
                await repository.GetByIdAsync(user.Id);

            Assert.NotNull(savedUser);
            Assert.Equal("New Name", savedUser.Name);
            Assert.Equal("new@example.com", savedUser.Email);
            Assert.Equal("newpassword", savedUser.PasswordHash);
            Assert.Equal("99999999", savedUser.Phone);
        }

        [Fact]
        public async Task Save_should_return_error_if_user_does_not_exist()
        {
            var repository = new UserRepository(DbContext);
            var handler = new SaveUserCommandHandler(repository);

            var command = new SaveUserCommand
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
            var validator = new SaveUserCommandValidator();

            var command = CreateValidCommand();
            command.Id = -1;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                x => x.PropertyName == "Id");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_error_when_name_is_invalid(string name)
        {
            var validator = new SaveUserCommandValidator();

            var command = CreateValidCommand();
            command.Name = name;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                x => x.PropertyName == "Name");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_error_when_email_is_empty(string email)
        {
            var validator = new SaveUserCommandValidator();

            var command = CreateValidCommand();
            command.Email = email;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                x => x.PropertyName == "Email");
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test@")]
        [InlineData("test.com")]
        public void SaveValidator_should_return_error_when_email_is_invalid(string email)
        {
            var validator = new SaveUserCommandValidator();

            var command = CreateValidCommand();
            command.Email = email;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                x => x.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_error_when_password_is_invalid(string password)
        {
            var validator = new SaveUserCommandValidator();

            var command = CreateValidCommand();
            command.PasswordHash = password;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                x => x.PropertyName == "PasswordHash");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_error_when_phone_is_invalid(string phone)
        {
            var validator = new SaveUserCommandValidator();

            var command = CreateValidCommand();
            command.Phone = phone;

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                x => x.PropertyName == "Phone");
        }

        [Fact]
        public void SaveValidator_should_return_true_when_command_is_valid()
        {
            var validator = new SaveUserCommandValidator();

            var command = CreateValidCommand();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
        }

        private SaveUserCommand CreateValidCommand()
        {
            return new SaveUserCommand
            {
                Id = 0,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "password123",
                Phone = "55512345"
            };
        }
    }
}