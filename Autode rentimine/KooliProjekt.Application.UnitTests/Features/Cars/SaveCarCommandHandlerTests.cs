using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Cars;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Cars
{
    public class SaveCarCommandHandlerTests : TestBase
    {
        [Fact]
        public void Save_should_throw_when_repository_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SaveCarCommandHandler(null));
        }

        [Fact]
        public async Task Save_should_throw_when_request_is_null()
        {
            var dbContext = DbContext;
            var repository = new CarRepository(dbContext);
            var handler = new SaveCarCommandHandler(repository);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Save_should_return_error_when_id_is_negative()
        {
            var dbContext = GetFaultyDbContext();
            var repository = new CarRepository(dbContext);
            var handler = new SaveCarCommandHandler(repository);

            var command = new SaveCarCommand
            {
                Id = -1
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.HasErrors);
            Assert.NotNull(result.PropertyErrors);
            Assert.True(result.PropertyErrors.ContainsKey("Id"));
        }

        [Fact]
        public async Task Save_should_save_new_car()
        {
            var dbContext = DbContext;
            var repository = new CarRepository(dbContext);
            var handler = new SaveCarCommandHandler(repository);

            var command = new SaveCarCommand
            {
                NumberPlate = "123ABC",
                TypeId = 1,
                Kmrate = 0.50m,
                TimeRate = 10m,
                IsAvailable = true
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var car = await repository.GetByIdAsync(1);

            Assert.NotNull(car);
            Assert.Equal("123ABC", car.NumberPlate);
            Assert.Equal(1, car.TypeId);
            Assert.Equal(0.50m, car.Kmrate);
            Assert.Equal(10m, car.TimeRate);
            Assert.True(car.IsAvailable);
        }

        [Fact]
        public async Task Save_should_save_existing_car()
        {
            var dbContext = DbContext;

            var car = new Car
            {
                NumberPlate = "OLD123",
                TypeId = 1,
                Kmrate = 1m,
                TimeRate = 5m,
                IsAvailable = false
            };

            await dbContext.Cars.AddAsync(car);
            await dbContext.SaveChangesAsync();

            var repository = new CarRepository(dbContext);
            var handler = new SaveCarCommandHandler(repository);

            var command = new SaveCarCommand
            {
                Id = car.Id,
                NumberPlate = "NEW123",
                TypeId = 2,
                Kmrate = 2.50m,
                TimeRate = 15m,
                IsAvailable = true
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var savedCar = await repository.GetByIdAsync(car.Id);

            Assert.NotNull(savedCar);
            Assert.Equal("NEW123", savedCar.NumberPlate);
            Assert.Equal(2, savedCar.TypeId);
            Assert.Equal(2.50m, savedCar.Kmrate);
            Assert.Equal(15m, savedCar.TimeRate);
            Assert.True(savedCar.IsAvailable);
        }

        [Fact]
        public async Task Save_should_return_error_if_car_does_not_exist()
        {
            var dbContext = DbContext;
            var repository = new CarRepository(dbContext);
            var handler = new SaveCarCommandHandler(repository);

            var command = new SaveCarCommand
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
            var validator = new SaveCarCommandValidator();

            var command = new SaveCarCommand
            {
                Id = -1,
                NumberPlate = "123ABC",
                TypeId = 1,
                Kmrate = 0.50m,
                TimeRate = 10m
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Id");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_error_when_number_plate_is_invalid(string numberPlate)
        {
            var validator = new SaveCarCommandValidator();

            var command = new SaveCarCommand
            {
                NumberPlate = numberPlate,
                TypeId = 1,
                Kmrate = 0.50m,
                TimeRate = 10m
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "NumberPlate");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SaveValidator_should_return_error_when_type_id_is_invalid(int typeId)
        {
            var validator = new SaveCarCommandValidator();

            var command = new SaveCarCommand
            {
                NumberPlate = "123ABC",
                TypeId = typeId,
                Kmrate = 0.50m,
                TimeRate = 10m
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "TypeId");
        }

        [Fact]
        public void SaveValidator_should_return_error_when_kmrate_is_negative()
        {
            var validator = new SaveCarCommandValidator();

            var command = new SaveCarCommand
            {
                NumberPlate = "123ABC",
                TypeId = 1,
                Kmrate = -1m,
                TimeRate = 10m
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Kmrate");
        }

        [Fact]
        public void SaveValidator_should_return_error_when_time_rate_is_negative()
        {
            var validator = new SaveCarCommandValidator();

            var command = new SaveCarCommand
            {
                NumberPlate = "123ABC",
                TypeId = 1,
                Kmrate = 0.50m,
                TimeRate = -1m
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "TimeRate");
        }

        [Fact]
        public void SaveValidator_should_return_true_when_command_is_valid()
        {
            var validator = new SaveCarCommandValidator();

            var command = new SaveCarCommand
            {
                Id = 0,
                NumberPlate = "123ABC",
                TypeId = 1,
                Kmrate = 0.50m,
                TimeRate = 10m,
                IsAvailable = true
            };

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
        }
    }
}