using System;
using System.Net.Http;
using KooliProjekt.Application.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace KooliProjekt.IntegrationTests.Helpers
{
    public abstract class TestBase : IDisposable
    {
        private ApplicationDbContext _dbContext;
        private IServiceScope _scope;

        public WebApplicationFactory<FakeStartup> Factory { get; }
        public HttpClient Client { get; }

        public TestBase()
        {
            Factory = new TestApplicationFactory<FakeStartup>();
            Client = Factory.CreateClient();
            _scope = Factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        protected ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext;
            }
        }

        public void Dispose()
        {
            try
            {
                var configuration = _scope.ServiceProvider.GetRequiredService<IConfiguration>();
                TestApplicationFactory<FakeStartup>.VerifyIsolatedTestDatabase(
                    _dbContext.Database.GetDbConnection().ConnectionString,
                    configuration);
                _dbContext.Database.EnsureDeleted();
            }
            finally
            {
                _scope.Dispose();
                Client.Dispose();
                Factory.Dispose();
            }
        }

        // Add your other helper methods here
    }
}