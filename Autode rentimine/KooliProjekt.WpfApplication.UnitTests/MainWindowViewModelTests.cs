using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using KooliProjekt.WpfApplication;
using KooliProjekt.WpfApplication.Api;
using Moq;

namespace KooliProjekt.WpfApplication.UnitTests
{
    public class MainWindowViewModelTests
    {
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly Mock<IDialogProvider> _dialogProviderMock;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModelTests()
        {
            _apiClientMock = new Mock<IApiClient>();
            _dialogProviderMock = new Mock<IDialogProvider>();
            _viewModel = new MainWindowViewModel(_apiClientMock.Object, _dialogProviderMock.Object);
        }

        [Fact]
        public void SelectedItem_should_return_correct_item()
        {
            var car = CreateCar(1, "TEST001");

            _viewModel.SelectedCar = car;

            Assert.Same(car, _viewModel.SelectedCar);
            Assert.Equal("TEST001", _viewModel.SelectedCar.NumberPlate);
        }

        [Fact]
        public void SelectedItem_should_call_notify_property_changed()
        {
            var car = CreateCar(1, "TEST001");
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(MainWindowViewModel.SelectedCar))
                {
                    propertyChangedRaised = true;
                }
            };

            _viewModel.SelectedCar = car;

            Assert.True(propertyChangedRaised);
            Assert.Same(car, _viewModel.SelectedCar);
        }

        [Fact]
        public async Task LoadData_should_load_data_from_api_client()
        {
            var cars = new List<Car>
            {
                CreateCar(1, "TEST001"),
                CreateCar(2, "TEST002")
            };
            _apiClientMock.Setup(client => client.List(null)).ReturnsAsync(cars);

            await _viewModel.LoadCarsAsync();

            _apiClientMock.Verify(client => client.List(null), Times.Once);
            Assert.Equal(2, _viewModel.Cars.Count);
            Assert.Equal(new[] { 1, 2 }, _viewModel.Cars.Select(car => car.Id));
            Assert.Same(cars[0], _viewModel.SelectedCar);
        }

        [Fact]
        public async Task LoadData_should_show_error_when_api_client_fails()
        {
            _apiClientMock.Setup(client => client.List(null))
                .ThrowsAsync(new HttpRequestException("API unavailable"));

            await _viewModel.LoadCarsAsync();

            _apiClientMock.Verify(client => client.List(null), Times.Once);
            Assert.Empty(_viewModel.Cars);
            _dialogProviderMock.Verify(dialog => dialog.ShowError(
                It.Is<string>(message => message.Contains("API unavailable"))), Times.Once);
        }

        [Fact]
        public void AddNew_Command_Should_Set_Empty_SelectedItem()
        {
            _viewModel.SelectedCar = CreateCar(7, "OLD007");

            _viewModel.AddNewCommand.Execute(null);

            Assert.NotNull(_viewModel.SelectedCar);
            Assert.Equal(0, _viewModel.SelectedCar.Id);
            Assert.Equal(string.Empty, _viewModel.SelectedCar.NumberPlate);
            Assert.Equal(0, _viewModel.SelectedCar.TypeId);
            Assert.Equal(0m, _viewModel.SelectedCar.Kmrate);
            Assert.Equal(0m, _viewModel.SelectedCar.TimeRate);
            Assert.True(_viewModel.SelectedCar.IsAvailable);
            Assert.True(_viewModel.AddNewCommand.CanExecute(null));
        }

        [Fact]
        public async Task SaveCommand_should_load_data_if_no_errors()
        {
            var car = CreateCar(0, "NEW001");
            car.TypeId = 2;
            _viewModel.SelectedCar = car;
            _viewModel.KmrateText = "0.50";
            _viewModel.TimeRateText = "10,00";
            var savedCar = CreateCar(12, "NEW001");
            savedCar.TypeId = 2;
            savedCar.Kmrate = 0.50m;
            savedCar.TimeRate = 10.00m;

            _apiClientMock.Setup(client => client.Save(It.IsAny<Car>()))
                .ReturnsAsync(new OperationResultResponse());
            _apiClientMock.Setup(client => client.List(null)).ReturnsAsync(new List<Car>());
            _apiClientMock.Setup(client => client.List("NEW001"))
                .ReturnsAsync(new List<Car> { savedCar });

            await ExecuteAndWaitAsync(_viewModel.SaveCommand, car);

            _apiClientMock.Verify(client => client.Save(It.Is<Car>(saved =>
                saved.NumberPlate == "NEW001" && saved.Kmrate == 0.50m && saved.TimeRate == 10.00m)), Times.Once);
            _apiClientMock.Verify(client => client.List(null), Times.Once);
            _apiClientMock.Verify(client => client.List("NEW001"), Times.Once);
            Assert.Single(_viewModel.Cars);
            Assert.Same(savedCar, _viewModel.Cars[0]);
            Assert.Same(savedCar, _viewModel.SelectedCar);
            _dialogProviderMock.Verify(dialog => dialog.ShowError(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SaveCommand_should_return_when_api_gave_error()
        {
            var car = CreateValidCar(1, "BAD001");
            _viewModel.SelectedCar = car;
            _apiClientMock.Setup(client => client.Save(car)).ReturnsAsync(new OperationResultResponse
            {
                HasErrors = true,
                Errors = new List<string> { "Save rejected" }
            });

            await ExecuteAndWaitAsync(_viewModel.SaveCommand, car);

            _apiClientMock.Verify(client => client.Save(car), Times.Once);
            _apiClientMock.Verify(client => client.List(It.IsAny<string>()), Times.Never);
            _dialogProviderMock.Verify(dialog => dialog.ShowError(
                It.Is<string>(message => message.Contains("Save rejected"))), Times.Once);
            Assert.Empty(_viewModel.Cars);
        }

        [Fact]
        public async Task SaveCommand_can_execute_when_selected_item_is_not_null()
        {
            var car = CreateValidCar(1, "BUSY001");
            var saveStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var finishSave = new TaskCompletionSource<OperationResultResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
            _viewModel.SelectedCar = car;
            _apiClientMock.Setup(client => client.Save(car))
                .Callback(() => saveStarted.TrySetResult())
                .Returns(finishSave.Task);
            _apiClientMock.Setup(client => client.List(null)).ReturnsAsync(new List<Car>());
            _apiClientMock.Setup(client => client.List("BUSY001")).ReturnsAsync(new List<Car> { car });

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
            Assert.True(_viewModel.SaveCommand.CanExecute(car));
            var commandCompletion = ExecuteAndWaitAsync(_viewModel.SaveCommand, car);
            await saveStarted.Task;
            Assert.False(_viewModel.SaveCommand.CanExecute(car));

            finishSave.SetResult(new OperationResultResponse());
            await commandCompletion;

            Assert.True(_viewModel.SaveCommand.CanExecute(car));
        }

        [Fact]
        public async Task DeleteCommand_should_return_when_no_confirmation()
        {
            var car = CreateCar(3, "NODEL001");
            _dialogProviderMock.Setup(dialog => dialog.ConfirmDelete("NODEL001")).Returns(false);

            await ExecuteAndWaitAsync(_viewModel.DeleteCommand, car);

            _dialogProviderMock.Verify(dialog => dialog.ConfirmDelete("NODEL001"), Times.Once);
            _apiClientMock.Verify(client => client.Delete(It.IsAny<int>()), Times.Never);
            _apiClientMock.Verify(client => client.List(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCommand_should_load_data_if_no_errors()
        {
            var car = CreateCar(3, "DELETE01");
            var refreshedCars = new List<Car> { CreateCar(4, "KEEP004") };
            _dialogProviderMock.Setup(dialog => dialog.ConfirmDelete("DELETE01")).Returns(true);
            _apiClientMock.Setup(client => client.Delete(3)).ReturnsAsync(new OperationResultResponse());
            _apiClientMock.Setup(client => client.List(null)).ReturnsAsync(refreshedCars);

            await ExecuteAndWaitAsync(_viewModel.DeleteCommand, car);

            _dialogProviderMock.Verify(dialog => dialog.ConfirmDelete("DELETE01"), Times.Once);
            _apiClientMock.Verify(client => client.Delete(3), Times.Once);
            _apiClientMock.Verify(client => client.List(null), Times.Once);
            Assert.Single(_viewModel.Cars);
            Assert.Same(refreshedCars[0], _viewModel.Cars[0]);
            Assert.Same(refreshedCars[0], _viewModel.SelectedCar);
        }

        [Fact]
        public async Task DeleteCommand_should_return_when_api_gave_error()
        {
            var car = CreateCar(3, "ERROR001");
            _dialogProviderMock.Setup(dialog => dialog.ConfirmDelete("ERROR001")).Returns(true);
            _apiClientMock.Setup(client => client.Delete(3)).ReturnsAsync(new OperationResultResponse
            {
                HasErrors = true,
                Errors = new List<string> { "Delete rejected" }
            });

            await ExecuteAndWaitAsync(_viewModel.DeleteCommand, car);

            _apiClientMock.Verify(client => client.Delete(3), Times.Once);
            _apiClientMock.Verify(client => client.List(It.IsAny<string>()), Times.Never);
            _dialogProviderMock.Verify(dialog => dialog.ShowError(
                It.Is<string>(message => message.Contains("Delete rejected"))), Times.Once);
            Assert.Empty(_viewModel.Cars);
        }

        [Fact]
        public async Task DeleteCommand_can_execute_when_selected_item_is_not_null_and_id_is_not_zero()
        {
            var car = CreateCar(5, "BUSYDEL5");
            var deleteStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var finishDelete = new TaskCompletionSource<OperationResultResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
            _dialogProviderMock.Setup(dialog => dialog.ConfirmDelete("BUSYDEL5")).Returns(true);
            _apiClientMock.Setup(client => client.Delete(5))
                .Callback(() => deleteStarted.TrySetResult())
                .Returns(finishDelete.Task);
            _apiClientMock.Setup(client => client.List(null)).ReturnsAsync(new List<Car>());

            Assert.False(_viewModel.DeleteCommand.CanExecute(null));
            Assert.False(_viewModel.DeleteCommand.CanExecute(CreateCar(0, "NEW000")));
            Assert.True(_viewModel.DeleteCommand.CanExecute(car));
            var commandCompletion = ExecuteAndWaitAsync(_viewModel.DeleteCommand, car);
            await deleteStarted.Task;
            Assert.False(_viewModel.DeleteCommand.CanExecute(car));

            finishDelete.SetResult(new OperationResultResponse());
            await commandCompletion;

            Assert.True(_viewModel.DeleteCommand.CanExecute(car));
        }

        private static Car CreateValidCar(int id, string numberPlate)
        {
            var car = CreateCar(id, numberPlate);
            car.TypeId = 1;
            return car;
        }

        private static Car CreateCar(int id, string numberPlate)
        {
            return new Car
            {
                Id = id,
                NumberPlate = numberPlate,
                TypeId = 1,
                Kmrate = 1.25m,
                TimeRate = 2.50m,
                IsAvailable = true
            };
        }

        private static Task ExecuteAndWaitAsync(System.Windows.Input.ICommand command, object parameter)
        {
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var observedDisabled = false;
            EventHandler handler = null;
            handler = (_, _) =>
            {
                var canExecute = command.CanExecute(parameter);
                if (!canExecute)
                {
                    observedDisabled = true;
                }
                else if (observedDisabled)
                {
                    command.CanExecuteChanged -= handler;
                    completion.TrySetResult();
                }
            };

            command.CanExecuteChanged += handler;
            command.Execute(parameter);
            return completion.Task;
        }
    }
}
