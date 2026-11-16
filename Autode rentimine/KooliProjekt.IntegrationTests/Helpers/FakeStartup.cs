using System;
using FluentValidation;
using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.WebAPI.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace KooliProjekt.IntegrationTests.Helpers
{
    public class FakeStartup 
    {
        public FakeStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var testConnectionString = Configuration.GetConnectionString("TestConnection");
            TestApplicationFactory<FakeStartup>.VerifyIsolatedTestDatabase(
                testConnectionString,
                Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(testConnectionString);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<ICarTypeRepository, CarTypeRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IInvoiceLineRepository, InvoiceLineRepository>();

            var applicationAssembly = typeof(ErrorHandlingBehavior<,>).Assembly;
            services.AddValidatorsFromAssembly(applicationAssembly);
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(applicationAssembly);
                config.AddOpenBehavior(typeof(ErrorHandlingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(TransactionalBehavior<,>));
            });

            services.AddControllersWithViews()
                    .AddApplicationPart(typeof(ApiControllerBase).Assembly);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                if (dbContext == null)
                {
                    throw new NullReferenceException("Cannot get instance of dbContext");
                }

                TestApplicationFactory<FakeStartup>.VerifyIsolatedTestDatabase(
                    dbContext.Database.GetDbConnection().ConnectionString,
                    Configuration);
                dbContext.Database.EnsureCreated();
            }
        }
    }
}