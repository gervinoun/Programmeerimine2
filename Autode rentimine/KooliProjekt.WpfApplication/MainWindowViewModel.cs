using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Windows.Input;
using KooliProjekt.WpfApplication.Api;
using Newtonsoft.Json;

namespace KooliProjekt.WpfApplication
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private readonly IApiClient _apiClient;
        private readonly IDialogProvider _dialogProvider;
        private readonly RelayCommand<object> _addNewCommand;
        private readonly RelayCommand<Car> _saveCommand;
        private readonly RelayCommand<Car> _deleteCommand;
        private Car _selectedCar;
        private string _errorMessage;
        private string _kmrateText = "0";
        private string _timeRateText = "0";
        private bool _isBusy;

        public MainWindowViewModel(IApiClient apiClient, IDialogProvider dialogProvider)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _dialogProvider = dialogProvider ?? throw new ArgumentNullException(nameof(dialogProvider));

            _addNewCommand = new RelayCommand<object>(
                _ =>
                {
                    AddNew();
                    return Task.CompletedTask;
                },
                _ => !IsBusy,
                HandleCommandException);
            _saveCommand = new RelayCommand<Car>(SaveAsync, car => !IsBusy && car != null, HandleCommandException);
            _deleteCommand = new RelayCommand<Car>(DeleteAsync, car => !IsBusy && car?.Id > 0, HandleCommandException);
        }

        public ObservableCollection<Car> Cars { get; } = new();

        public ICommand AddNewCommand => _addNewCommand;
        public ICommand SaveCommand => _saveCommand;
        public ICommand DeleteCommand => _deleteCommand;

        public Car SelectedCar
        {
            get => _selectedCar;
            set
            {
                if (!ReferenceEquals(_selectedCar, value))
                {
                    _selectedCar = value;
                    NotifyPropertyChanged();
                    SetRateEditorValues(value);
                    RaiseCommandCanExecuteChanged();
                }
            }
        }

        public string KmrateText
        {
            get => _kmrateText;
            set
            {
                if (_kmrateText != value)
                {
                    _kmrateText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string TimeRateText
        {
            get => _timeRateText;
            set
            {
                if (_timeRateText != value)
                {
                    _timeRateText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    NotifyPropertyChanged();
                    RaiseCommandCanExecuteChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public async Task LoadCarsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            try
            {
                await LoadCarsCoreAsync();
            }
            catch (HttpRequestException exception)
            {
                ShowError($"Unable to load cars from http://localhost:5086/api/Cars. Make sure the WebAPI is running.\n\n{exception.Message}");
            }
            catch (TaskCanceledException)
            {
                ShowError("The request to load cars timed out. Make sure the WebAPI is running at http://localhost:5086.");
            }
            catch (JsonException exception)
            {
                ShowError($"The API returned an unreadable Cars response.\n\n{exception.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ShowError(string message)
        {
            ErrorMessage = message;
            _dialogProvider.ShowError(message);
        }

        private void AddNew()
        {
            ErrorMessage = string.Empty;
            SelectedCar = new Car
            {
                Id = 0,
                NumberPlate = string.Empty,
                TypeId = 0,
                Kmrate = 0,
                TimeRate = 0,
                IsAvailable = true
            };
        }

        private async Task SaveAsync(Car car)
        {
            if (!TryParseRate(KmrateText, "Km rate", out var kmrate)
                || !TryParseRate(TimeRateText, "Time rate", out var timeRate))
            {
                return;
            }

            if (!ValidateCar(car, kmrate, timeRate))
            {
                return;
            }

            if (IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                car.NumberPlate = car.NumberPlate.Trim();
                car.Kmrate = kmrate;
                car.TimeRate = timeRate;
                var result = await _apiClient.Save(car);
                if (ShowApiErrors(result))
                {
                    return;
                }

                await LoadCarsCoreAsync(car.NumberPlate);
            }
            catch (HttpRequestException exception)
            {
                ShowError($"The API could not save the car.\n\n{exception.Message}");
            }
            catch (TaskCanceledException)
            {
                ShowError("The request to save the car timed out.");
            }
            catch (JsonException exception)
            {
                ShowError($"The API returned an unreadable Save response.\n\n{exception.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteAsync(Car car)
        {
            if (car == null || car.Id <= 0)
            {
                ShowError("Select an existing car to delete.");
                return;
            }

            if (IsBusy || !_dialogProvider.ConfirmDelete(car.NumberPlate))
            {
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                var result = await _apiClient.Delete(car.Id);
                if (ShowApiErrors(result))
                {
                    return;
                }

                await LoadCarsCoreAsync();
            }
            catch (HttpRequestException exception)
            {
                ShowError($"The API could not delete the car.\n\n{exception.Message}");
            }
            catch (TaskCanceledException)
            {
                ShowError("The request to delete the car timed out.");
            }
            catch (JsonException exception)
            {
                ShowError($"The API returned an unreadable Delete response.\n\n{exception.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadCarsCoreAsync(string numberPlateToSelect = null)
        {
            ErrorMessage = string.Empty;
            var cars = (await _apiClient.List())?.ToList() ?? new List<Car>();
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
                        cars.Add(carToSelect);
                    }
                }
            }

            Cars.Clear();
            foreach (var item in cars)
            {
                Cars.Add(item);
            }

            SelectedCar = carToSelect ?? Cars.FirstOrDefault();
        }

        private bool ValidateCar(Car car, decimal kmrate, decimal timeRate)
        {
            if (car == null)
            {
                ShowError("Select or create a car before saving.");
                return false;
            }

            if (car.Id < 0)
            {
                ShowError("Car ID must be zero or greater.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(car.NumberPlate))
            {
                ShowError("Number plate is required.");
                return false;
            }

            if (car.TypeId <= 0)
            {
                ShowError("Type ID must be a whole number greater than zero.");
                return false;
            }

            if (kmrate < 0)
            {
                ShowError("Km rate must be a non-negative number.");
                return false;
            }

            if (timeRate < 0)
            {
                ShowError("Time rate must be a non-negative number.");
                return false;
            }

            return true;
        }

        private bool TryParseRate(string input, string fieldName, out decimal value)
        {
            var normalized = input?.Trim();
            if (string.IsNullOrEmpty(normalized)
                || normalized.EndsWith(",", StringComparison.Ordinal)
                || normalized.EndsWith(".", StringComparison.Ordinal)
                || (normalized.Contains(',') && normalized.Contains('.')))
            {
                value = 0;
                ShowError($"{fieldName} must be a complete decimal number. Use either a comma or a dot as the decimal separator.");
                return false;
            }

            normalized = normalized.Replace(',', '.');
            if (!decimal.TryParse(
                normalized,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out value))
            {
                ShowError($"{fieldName} must be a valid decimal number. Use either a comma or a dot as the decimal separator.");
                return false;
            }

            return true;
        }

        private void SetRateEditorValues(Car car)
        {
            KmrateText = car?.Kmrate.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
            TimeRateText = car?.TimeRate.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
        }

        private bool ShowApiErrors(OperationResultResponse result)
        {
            if (result == null)
            {
                ShowError("The API returned an empty response.");
                return true;
            }

            var messages = new List<string>();
            if (result.Errors != null)
            {
                messages.AddRange(result.Errors);
            }

            if (result.PropertyErrors != null)
            {
                messages.AddRange(result.PropertyErrors.Select(error => $"{error.Key}: {error.Value}"));
            }

            if (!result.HasErrors && messages.Count == 0)
            {
                return false;
            }

            ShowError(messages.Count > 0
                ? string.Join(Environment.NewLine, messages)
                : "The API rejected the request.");
            return true;
        }

        private void HandleCommandException(Exception exception)
        {
            ShowError($"The command could not be completed.\n\n{exception.Message}");
        }

        private void RaiseCommandCanExecuteChanged()
        {
            _addNewCommand.RaiseCanExecuteChanged();
            _saveCommand.RaiseCanExecuteChanged();
            _deleteCommand.RaiseCanExecuteChanged();
        }

    }
}
