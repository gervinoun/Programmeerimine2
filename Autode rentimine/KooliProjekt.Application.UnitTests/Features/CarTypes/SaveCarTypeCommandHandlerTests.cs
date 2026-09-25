using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.CarTypes;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.CarTypes
{
    public class SaveCarTypeCommandHandlerTests : TestBase
    {
        [Fact]
        public async Task Save_should_add_new_car_type()
        {
            var repository = new CarTypeRepository(DbContext);
            var handler = new SaveCarTypeCommandHandler(repository);

            var command = new SaveCarTypeCommand
            {
                Id = 0,
                Name = "SUV"
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var carType = await repository.GetByIdAsync(1);

            Assert.NotNull(carType);
            Assert.Equal("SUV", carType.Name);
        }

        [Fact]
        public async Task Save_should_update_existing_car_type()
        {
            var repository = new CarTypeRepository(DbContext);

            var carType = new CarType
            {
                Name = "Old name"
            };

            await repository.AddAsync(carType);
            await repository.SaveChangesAsync();

            var handler = new SaveCarTypeCommandHandler(repository);

            var command = new SaveCarTypeCommand
            {
                Id = carType.Id,
                Name = "Sedan"
            };

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.HasErrors);

            var savedCarType =
                await repository.GetByIdAsync(carType.Id);

            Assert.NotNull(savedCarType);
            Assert.Equal("Sedan", savedCarType.Name);
        }

        [Fact]
        public async Task Save_should_return_error_if_car_type_does_not_exist()
        {
            var repository = new CarTypeRepository(DbContext);
            var handler = new SaveCarTypeCommandHandler(repository);

            var command = new SaveCarTypeCommand
            {
                Id = 999,
                Name = "SUV"
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
            var validator = new SaveCarTypeCommandValidator();

            var command = new SaveCarTypeCommand
            {
                Id = -1,
                Name = "SUV"
            };

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
            var validator = new SaveCarTypeCommandValidator();

            var command = new SaveCarTypeCommand
            {
                Id = 0,
                Name = name
            };

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                x => x.PropertyName == "Name");
        }

        [Fact]
        public void SaveValidator_should_return_true_when_command_is_valid()
        {
            var validator = new SaveCarTypeCommandValidator();

            var command = new SaveCarTypeCommand
            {
                Id = 0,
                Name = "SUV"
            };

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
        }
    }
}