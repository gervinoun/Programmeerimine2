using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Users;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    public class UsersControllerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_user_when_id_exists()
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

            using var response = await Client.GetAsync($"/api/Users/{user.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.True(result.Value.HasValue);
            var value = result.Value.Value;
            Assert.Equal(user.Id, value.GetProperty("id").GetInt32());
            Assert.Equal("Alice Example", value.GetProperty("name").GetString());
            Assert.Equal("alice@example.com", value.GetProperty("email").GetString());
            Assert.Equal("555-0100", value.GetProperty("phone").GetString());
        }

        [Fact]
        public async Task Get_should_return_null_value_when_id_is_missing()
        {
            using var response = await Client.GetAsync("/api/Users/9999");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult<JsonElement?>>();
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task List_should_return_all_users_and_filter_by_name_or_email()
        {
            DbContext.Users.AddRange(
                new User { Name = "Alice Example", Email = "alice@example.com", PasswordHash = "hash-1" },
                new User { Name = "Bob Sample", Email = "bob@sample.com", PasswordHash = "hash-2" });
            await DbContext.SaveChangesAsync();

            using var allResponse = await Client.GetAsync("/api/Users");
            Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
            var allUsers = await allResponse.Content.ReadFromJsonAsync<List<User>>();
            Assert.NotNull(allUsers);
            Assert.Equal(2, allUsers.Count);

            using var nameResponse = await Client.GetAsync("/api/Users?name=ali");
            Assert.Equal(HttpStatusCode.OK, nameResponse.StatusCode);
            var byName = await nameResponse.Content.ReadFromJsonAsync<List<User>>();
            Assert.NotNull(byName);
            Assert.Equal("Alice Example", Assert.Single(byName).Name);

            using var emailResponse = await Client.GetAsync("/api/Users?email=EXAMPLE.COM");
            Assert.Equal(HttpStatusCode.OK, emailResponse.StatusCode);
            var byEmail = await emailResponse.Content.ReadFromJsonAsync<List<User>>();
            Assert.NotNull(byEmail);
            Assert.Equal("alice@example.com", Assert.Single(byEmail).Email);
        }

        [Fact]
        public async Task Save_should_create_a_new_user()
        {
            var result = await PostSaveAsync(new SaveUserCommand
            {
                Name = "New User",
                Email = "new@example.com",
                PasswordHash = "hash-new",
                Phone = "555-0101"
            });

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.Users.CountAsync());
            var savedUser = await DbContext.Users.SingleAsync();
            Assert.Equal("New User", savedUser.Name);
            Assert.Equal("new@example.com", savedUser.Email);
            Assert.Equal("hash-new", savedUser.PasswordHash);
            Assert.Equal("555-0101", savedUser.Phone);
        }

        [Fact]
        public async Task Save_should_update_an_existing_user()
        {
            var user = new User
            {
                Name = "Old User",
                Email = "old@example.com",
                PasswordHash = "old-hash",
                Phone = "555-0100"
            };
            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var result = await PostSaveAsync(new SaveUserCommand
            {
                Id = user.Id,
                Name = "Updated User",
                Email = "updated@example.com",
                PasswordHash = "updated-hash",
                Phone = "555-0102"
            });

            Assert.False(result.HasErrors);
            await DbContext.Entry(user).ReloadAsync();
            Assert.Equal(1, await DbContext.Users.CountAsync());
            Assert.Equal("Updated User", user.Name);
            Assert.Equal("updated@example.com", user.Email);
            Assert.Equal("updated-hash", user.PasswordHash);
            Assert.Equal("555-0102", user.Phone);
        }

        [Fact]
        public async Task Save_should_return_error_for_a_missing_user_id()
        {
            var result = await PostSaveAsync(new SaveUserCommand
            {
                Id = 9999,
                Name = "Missing User",
                Email = "missing@example.com",
                PasswordHash = "hash",
                Phone = "555-0103"
            });

            Assert.True(result.HasErrors);
            Assert.Contains("User not found", result.Errors);
            Assert.Empty(await DbContext.Users.ToListAsync());
        }

        [Fact]
        public async Task Save_should_return_validation_error_for_an_invalid_email()
        {
            var result = await PostSaveAsync(new SaveUserCommand
            {
                Name = "Invalid User",
                Email = "invalid-email",
                PasswordHash = "hash",
                Phone = "555-0104"
            });

            Assert.True(result.HasErrors);
            Assert.Equal("Email must be valid", result.PropertyErrors[nameof(SaveUserCommand.Email)]);
            Assert.Empty(await DbContext.Users.ToListAsync());
        }

        [Fact]
        public async Task Delete_should_remove_an_existing_user()
        {
            var user = new User
            {
                Name = "Delete User",
                Email = "delete@example.com",
                PasswordHash = "hash",
                Phone = "555-0105"
            };
            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(user.Id);

            Assert.False(result.HasErrors);
            Assert.False(await DbContext.Users.AnyAsync(x => x.Id == user.Id));
        }

        [Fact]
        public async Task Delete_should_leave_database_unchanged_for_a_missing_user()
        {
            var user = new User
            {
                Name = "Keep User",
                Email = "keep@example.com",
                PasswordHash = "hash",
                Phone = "555-0106"
            };
            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var result = await DeleteAsync(9999);

            Assert.False(result.HasErrors);
            Assert.Equal(1, await DbContext.Users.CountAsync());
            Assert.True(await DbContext.Users.AnyAsync(x => x.Id == user.Id));
        }

        private async Task<OperationResultResponse> PostSaveAsync(SaveUserCommand command)
        {
            using var response = await Client.PostAsJsonAsync("/api/Users", command);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }

        private async Task<OperationResultResponse> DeleteAsync(int id)
        {
            using var response = await Client.DeleteAsync($"/api/Users/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResultResponse>();
            Assert.NotNull(result);
            return result;
        }
    }
}
