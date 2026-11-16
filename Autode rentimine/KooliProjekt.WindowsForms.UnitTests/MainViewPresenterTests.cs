using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using KooliProjekt.WindowsForms;
using KooliProjekt.WindowsForms.Api;
using Moq;
using Xunit;

namespace KooliProjekt.WindowsForms.UnitTests
{
    public class MainViewPresenterTests
    {
        [Fact]
        public async Task LoadData_should_call_ShowError_with_faulty_response()
        {
            var view = new Mock<IMainView>();
            var apiClient = new Mock<IApiClient>();
            apiClient.Setup(client => client.List((string)null))
                .ThrowsAsync(new HttpRequestException("connection failed"));
            var presenter = new MainViewPresenter(view.Object, apiClient.Object);

            await presenter.LoadData();

            view.Verify(view => view.ShowError(It.Is<string>(message =>
                message.Contains("Unable to load cars") && message.Contains("connection failed"))), Times.Once);
            view.VerifySet(view => view.DataSource = It.IsAny<IList<Car>>(), Times.Never);
            apiClient.Verify(client => client.List(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoadData_should_set_DataSource_with_valid_response()
        {
            var firstCar = CreateCar();
            var cars = new List<Car> { firstCar, CreateCar() };
            var view = new Mock<IMainView>();
            view.SetupProperty(view => view.DataSource);
            view.SetupProperty(view => view.SelectedItem);
            view.SetupProperty(view => view.Id);
            view.SetupProperty(view => view.NumberPlate);
            view.SetupProperty(view => view.TypeId);
            view.SetupProperty(view => view.Kmrate);
            view.SetupProperty(view => view.TimeRate);
            view.SetupProperty(view => view.IsAvailable);
            var apiClient = new Mock<IApiClient>();
            apiClient.Setup(client => client.List((string)null)).ReturnsAsync(cars);
            var presenter = new MainViewPresenter(view.Object, apiClient.Object);

            await presenter.LoadData();

            apiClient.Verify(client => client.List((string)null), Times.Once);
            view.VerifySet(view => view.DataSource = cars, Times.Once);
            view.VerifySet(view => view.SelectedItem = firstCar, Times.Once);
            Assert.Same(cars, view.Object.DataSource);
            Assert.Equal(firstCar.Id.ToString(CultureInfo.InvariantCulture), view.Object.Id);
            Assert.Equal(firstCar.NumberPlate, view.Object.NumberPlate);
        }

        [Fact]
        public void SetSelection_should_clear_fields_with_null_selection()
        {
            var view = new Mock<IMainView>();
            var presenter = new MainViewPresenter(view.Object, new Mock<IApiClient>().Object);

            presenter.SetSelection(null);

            view.VerifySet(view => view.Id = string.Empty, Times.Once);
            view.VerifySet(view => view.NumberPlate = string.Empty, Times.Once);
            view.VerifySet(view => view.TypeId = string.Empty, Times.Once);
            view.VerifySet(view => view.Kmrate = string.Empty, Times.Once);
            view.VerifySet(view => view.TimeRate = string.Empty, Times.Once);
            view.VerifySet(view => view.IsAvailable = false, Times.Once);
        }

        [Fact]
        public void SetSelection_should_set_fields_with_valid_selection()
        {
            var car = CreateCar();
            var view = new Mock<IMainView>();
            var presenter = new MainViewPresenter(view.Object, new Mock<IApiClient>().Object);

            presenter.SetSelection(car);

            view.VerifySet(view => view.Id = car.Id.ToString(CultureInfo.InvariantCulture), Times.Once);
            view.VerifySet(view => view.NumberPlate = car.NumberPlate, Times.Once);
            view.VerifySet(view => view.TypeId = car.TypeId.ToString(CultureInfo.InvariantCulture), Times.Once);
            view.VerifySet(view => view.Kmrate = car.Kmrate.ToString(CultureInfo.CurrentCulture), Times.Once);
            view.VerifySet(view => view.TimeRate = car.TimeRate.ToString(CultureInfo.CurrentCulture), Times.Once);
            view.VerifySet(view => view.IsAvailable = car.IsAvailable, Times.Once);
        }

        [Fact]
        public async Task Save_should_call_ShowError_with_faulty_response()
        {
            var view = new Mock<IMainView>();
            SetupValidCarFields(view, string.Empty);
            var apiClient = new Mock<IApiClient>();
            apiClient.Setup(client => client.Save(It.IsAny<Car>())).ReturnsAsync(new OperationResultResponse
            {
                HasErrors = true,
                Errors = new List<string> { "Save rejected" },
                PropertyErrors = new Dictionary<string, string> { ["NumberPlate"] = "Plate is invalid" }
            });
            var presenter = new MainViewPresenter(view.Object, apiClient.Object);

            await presenter.Save();

            apiClient.Verify(client => client.Save(It.Is<Car>(car =>
                car.Id == 0 && car.NumberPlate == "TEST123" && car.TypeId == 1)), Times.Once);
            view.Verify(view => view.ShowError(It.Is<string>(message =>
                message.Contains("Save rejected") && message.Contains("NumberPlate: Plate is invalid"))), Times.Once);
            apiClient.Verify(client => client.List(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Save_should_call_LoadData_with_valid_response()
        {
            var view = new Mock<IMainView>();
            SetupValidCarFields(view, "17");
            view.SetupProperty(view => view.DataSource);
            view.SetupProperty(view => view.SelectedItem);
            var savedCar = CreateCar(17, "TEST123");
            var cars = new List<Car> { savedCar };
            var apiClient = new Mock<IApiClient>();
            apiClient.Setup(client => client.Save(It.IsAny<Car>()))
                .ReturnsAsync(new OperationResultResponse());
            apiClient.Setup(client => client.List((string)null)).ReturnsAsync(cars);
            var presenter = new MainViewPresenter(view.Object, apiClient.Object);

            await presenter.Save();

            apiClient.Verify(client => client.Save(It.Is<Car>(car =>
                car.Id == 17 && car.NumberPlate == "TEST123" && car.TypeId == 1)), Times.Once);
            apiClient.Verify(client => client.List((string)null), Times.Once);
            view.VerifySet(view => view.DataSource = cars, Times.Once);
            view.VerifySet(view => view.SelectedItem = savedCar, Times.Once);
            Assert.Equal("TEST123", view.Object.NumberPlate);
        }

        [Fact]
        public async Task Delete_should_return_when_user_didnot_confirmed()
        {
            var view = new Mock<IMainView>();
            view.SetupGet(view => view.Id).Returns("17");
            view.Setup(view => view.ConfirmDelete()).Returns(false);
            var apiClient = new Mock<IApiClient>();
            var presenter = new MainViewPresenter(view.Object, apiClient.Object);

            await presenter.Delete();

            view.Verify(view => view.ConfirmDelete(), Times.Once);
            apiClient.Verify(client => client.Delete(It.IsAny<int>()), Times.Never);
            apiClient.Verify(client => client.List(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_should_call_ShowError_with_faulty_response()
        {
            var view = new Mock<IMainView>();
            view.SetupGet(view => view.Id).Returns("17");
            view.Setup(view => view.ConfirmDelete()).Returns(true);
            var apiClient = new Mock<IApiClient>();
            apiClient.Setup(client => client.Delete(17)).ReturnsAsync(new OperationResultResponse
            {
                HasErrors = true,
                Errors = new List<string> { "Delete rejected" }
            });
            var presenter = new MainViewPresenter(view.Object, apiClient.Object);

            await presenter.Delete();

            apiClient.Verify(client => client.Delete(17), Times.Once);
            view.Verify(view => view.ShowError(It.Is<string>(message => message.Contains("Delete rejected"))), Times.Once);
            apiClient.Verify(client => client.List(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_should_call_LoadData_with_valid_response()
        {
            var view = new Mock<IMainView>();
            view.SetupProperty(view => view.Id, "17");
            view.SetupProperty(view => view.NumberPlate, "CAR017");
            view.SetupProperty(view => view.TypeId, "2");
            view.SetupProperty(view => view.Kmrate, "0.75");
            view.SetupProperty(view => view.TimeRate, "12.5");
            view.SetupProperty(view => view.IsAvailable, true);
            view.Setup(view => view.ConfirmDelete()).Returns(true);
            view.SetupProperty(view => view.DataSource);
            view.SetupProperty(view => view.SelectedItem);
            var cars = new List<Car> { CreateCar(18, "OTHER123") };
            var apiClient = new Mock<IApiClient>();
            apiClient.Setup(client => client.Delete(17)).ReturnsAsync(new OperationResultResponse());
            apiClient.Setup(client => client.List((string)null)).ReturnsAsync(cars);
            var presenter = new MainViewPresenter(view.Object, apiClient.Object);

            await presenter.Delete();

            view.Verify(view => view.ConfirmDelete(), Times.Once);
            apiClient.Verify(client => client.Delete(17), Times.Once);
            apiClient.Verify(client => client.List((string)null), Times.Once);
            view.VerifySet(view => view.DataSource = cars, Times.Once);
            view.VerifySet(view => view.SelectedItem = null, Times.Once);
            Assert.Null(view.Object.SelectedItem);
            Assert.Equal(string.Empty, view.Object.Id);
            Assert.Equal(string.Empty, view.Object.NumberPlate);
            Assert.Equal(string.Empty, view.Object.TypeId);
            Assert.Equal(string.Empty, view.Object.Kmrate);
            Assert.Equal(string.Empty, view.Object.TimeRate);
            Assert.False(view.Object.IsAvailable);
        }

        private static void SetupValidCarFields(Mock<IMainView> view, string id)
        {
            view.SetupGet(view => view.Id).Returns(id);
            view.SetupGet(view => view.NumberPlate).Returns("TEST123");
            view.SetupGet(view => view.TypeId).Returns("1");
            view.SetupGet(view => view.Kmrate).Returns(0.5m.ToString(CultureInfo.CurrentCulture));
            view.SetupGet(view => view.TimeRate).Returns(10m.ToString(CultureInfo.CurrentCulture));
            view.SetupGet(view => view.IsAvailable).Returns(true);
        }

        private static Car CreateCar(int id = 13, string numberPlate = "CAR013")
        {
            return new Car
            {
                Id = id,
                NumberPlate = numberPlate,
                TypeId = 2,
                Kmrate = 0.75m,
                TimeRate = 12.5m,
                IsAvailable = true
            };
        }
    }
}
