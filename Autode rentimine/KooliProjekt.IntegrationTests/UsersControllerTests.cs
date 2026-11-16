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
    }
}
