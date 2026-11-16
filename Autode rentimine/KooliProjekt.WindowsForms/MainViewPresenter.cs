using System.Globalization;
using System.Net.Http;
using KooliProjekt.WindowsForms.Api;
using Newtonsoft.Json;

namespace KooliProjekt.WindowsForms
{
    public class MainViewPresenter
    {
        private readonly IMainView _view;
        private readonly IApiClient _apiClient;

        public MainViewPresenter(IMainView view, IApiClient apiClient)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task LoadData(string numberPlateToSelect = null)
        {
            try
            {
                var cars = await _apiClient.List() ?? new List<Car>();
                Car carToSelect = null;

                if (!string.IsNullOrWhiteSpace(numberPlateToSelect))
                {
                    carToSelect = cars.FirstOrDefault(car =>
                        string.Equals(car.NumberPlate, numberPlateToSelect, StringComparison.OrdinalIgnoreCase));

                    if (carToSelect == null)
                    {
                        var matchingCars = await _apiClient.List(numberPlateToSelect);
                        carToSelect = matchingCars?.FirstOrDefault(car =>
                            string.Equals(car.NumberPlate, numberPlateToSelect, StringComparison.OrdinalIgnoreCase));

                        if (carToSelect != null)
                        {
                            cars = cars.Append(carToSelect).ToList();
                        }
                    }
                }

                _view.DataSource = cars;
                carToSelect ??= cars.FirstOrDefault();
                _view.SelectedItem = carToSelect;
                SetSelection(carToSelect);
            }
            catch (HttpRequestException exception)
            {
                _view.ShowError($"Unable to load cars. Make sure the WebAPI is running at http://localhost:5086.\n\n{exception.Message}");
            }
            catch (TaskCanceledException)
            {
                _view.ShowError("The request to load cars timed out. Make sure the WebAPI is running at http://localhost:5086.");
            }
            catch (JsonException exception)
            {
                _view.ShowError($"The API returned an unreadable Cars response.\n\n{exception.Message}");
            }
        }

        public void SetSelection(Car car)
        {
            _view.Id = car?.Id.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            _view.NumberPlate = car?.NumberPlate ?? string.Empty;
            _view.TypeId = car?.TypeId.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            _view.Kmrate = car?.Kmrate.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            _view.TimeRate = car?.TimeRate.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            _view.IsAvailable = car?.IsAvailable ?? false;
        }
    }
}
