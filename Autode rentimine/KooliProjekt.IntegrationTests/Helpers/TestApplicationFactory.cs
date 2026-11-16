using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace KooliProjekt.IntegrationTests.Helpers
{
    public class TestApplicationFactory<TTestStartup> : WebApplicationFactory<TTestStartup> where TTestStartup : class
    {
        private readonly string _testConnectionString;

        public TestApplicationFactory()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false)
                .Build();
            var connectionString = configuration.GetConnectionString("TestConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("The integration test connection string is missing.");
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(connectionStringBuilder.InitialCatalog))
            {
                throw new InvalidOperationException("The integration test database name is missing.");
            }

            connectionStringBuilder.InitialCatalog =
                $"{connectionStringBuilder.InitialCatalog}-{Guid.NewGuid():N}";
            _testConnectionString = connectionStringBuilder.ConnectionString;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var host = Host.CreateDefaultBuilder()
                            .ConfigureWebHost(builder =>
                            {
                                builder.UseContentRoot(AppContext.BaseDirectory);
                                builder.ConfigureAppConfiguration((c, b) =>
                                {
                                    c.HostingEnvironment.ApplicationName = "KooliProjekt.WebAPI";
                                });
                                builder.UseStartup<TTestStartup>();
                            })
                            .ConfigureAppConfiguration((context, conf) =>
                            {
                                var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                                conf.AddJsonFile(configPath, optional: false, reloadOnChange: false);
                                conf.AddInMemoryCollection(new Dictionary<string, string>
                                {
                                    ["ConnectionStrings:TestConnection"] = _testConnectionString
                                });
                            });
            return host;
        }

        public static void VerifyIsolatedTestDatabase(
            string actualConnectionString,
            IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(actualConnectionString))
            {
                throw new InvalidOperationException("The database connection string is missing.");
            }

            var configuredTestConnection = configuration.GetConnectionString("TestConnection");
            if (string.IsNullOrWhiteSpace(configuredTestConnection))
            {
                throw new InvalidOperationException("The integration test connection string is missing.");
            }

            var actual = new SqlConnectionStringBuilder(actualConnectionString);
            var configuredTest = new SqlConnectionStringBuilder(configuredTestConnection);
            const string expectedDatabasePrefix = "aspnet-KooliProjekt-Integration-";

            if (!actual.InitialCatalog.StartsWith(expectedDatabasePrefix, StringComparison.OrdinalIgnoreCase)
                || !Guid.TryParseExact(actual.InitialCatalog.Substring(expectedDatabasePrefix.Length), "N", out _))
            {
                throw new InvalidOperationException("Refusing database operation outside a uniquely named integration test database.");
            }

            if (!string.Equals(
                    actual.ConnectionString,
                    configuredTest.ConnectionString,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The active database connection does not match TestConnection.");
            }

            var defaultConnection = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(defaultConnection))
            {
                var application = new SqlConnectionStringBuilder(defaultConnection);
                if (string.Equals(actual.DataSource, application.DataSource, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(actual.InitialCatalog, application.InitialCatalog, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Refusing database operation on DefaultConnection.");
                }
            }
        }
    }
}
