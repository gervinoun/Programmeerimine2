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

        public void AddNew()
        {
            _view.SelectedItem = null;
            SetSelection(null);
            _view.Kmrate = "0";
            _view.TimeRate = "0";
            _view.IsAvailable = true;
            _view.FocusNumberPlate();
        }

        public async Task Save()
        {
            if (!TryBuildCar(out var car, out var validationMessage))
            {
                _view.ShowError(validationMessage);
                return;
            }

            try
            {
                var result = await _apiClient.Save(car);
                if (ShowApiErrors(result))
                {
                    return;
                }

                await LoadData(car.NumberPlate);
            }
            catch (HttpRequestException exception)
            {
                _view.ShowError($"The API could not save the car.\n\n{exception.Message}");
            }
            catch (TaskCanceledException)
            {
                _view.ShowError("The request to save the car timed out.");
            }
            catch (JsonException exception)
            {
                _view.ShowError($"The API returned an unreadable Save response.\n\n{exception.Message}");
            }
        }

        public async Task Delete()
        {
            if (!int.TryParse(_view.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) || id <= 0)
            {
                _view.ShowError("Select a saved car to delete.");
                return;
            }

            if (!_view.ConfirmDelete())
            {
                return;
            }

            try
            {
                var result = await _apiClient.Delete(id);
                if (ShowApiErrors(result))
                {
                    return;
                }

                await LoadData();
                _view.SelectedItem = null;
                SetSelection(null);
            }
            catch (HttpRequestException exception)
            {
                _view.ShowError($"The API could not delete the car.\n\n{exception.Message}");
            }
            catch (TaskCanceledException)
            {
                _view.ShowError("The request to delete the car timed out.");
            }
            catch (JsonException exception)
            {
                _view.ShowError($"The API returned an unreadable Delete response.\n\n{exception.Message}");
            }
        }

        private bool TryBuildCar(out Car car, out string validationMessage)
        {
            car = null;
            validationMessage = null;

            var id = 0;
            if (!string.IsNullOrWhiteSpace(_view.Id) &&
                !int.TryParse(_view.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                validationMessage = "Car ID must be a whole number.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(_view.NumberPlate))
            {
                validationMessage = "Number plate is required.";
                return false;
            }

            if (!int.TryParse(_view.TypeId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var typeId) || typeId <= 0)
            {
                validationMessage = "Type ID must be a whole number greater than zero.";
                return false;
            }

            if (!decimal.TryParse(_view.Kmrate, NumberStyles.Number, CultureInfo.CurrentCulture, out var kmRate) || kmRate < 0)
            {
                validationMessage = "Km rate must be a non-negative number.";
                return false;
            }

            if (!decimal.TryParse(_view.TimeRate, NumberStyles.Number, CultureInfo.CurrentCulture, out var timeRate) || timeRate < 0)
            {
                validationMessage = "Time rate must be a non-negative number.";
                return false;
            }

            car = new Car
            {
                Id = id,
                NumberPlate = _view.NumberPlate.Trim(),
                TypeId = typeId,
                Kmrate = kmRate,
                TimeRate = timeRate,
                IsAvailable = _view.IsAvailable
            };
            return true;
        }

        private bool ShowApiErrors(OperationResultResponse result)
        {
            if (result == null)
            {
                _view.ShowError("The API returned an empty response.");
                return true;
            }

            if (!result.HasErrors)
            {
                return false;
            }

            var messages = new List<string>();
            if (result.Errors != null)
            {
                messages.AddRange(result.Errors);
            }

            if (result.PropertyErrors != null)
            {
                foreach (var error in result.PropertyErrors)
                {
                    messages.Add($"{error.Key}: {error.Value}");
                }
            }

            _view.ShowError(messages.Count > 0 ? string.Join(Environment.NewLine, messages) : "The API rejected the request.");
            return true;
        }
    }
}
