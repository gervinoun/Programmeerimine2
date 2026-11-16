using System.Collections.ObjectModel;
using System.Net.Http;
using KooliProjekt.WpfApplication.Api;
using Newtonsoft.Json;

namespace KooliProjekt.WpfApplication
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private readonly IApiClient _apiClient;
        private Car _selectedCar;
        private string _errorMessage;

        public MainWindowViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public ObservableCollection<Car> Cars { get; } = new();

        public Car SelectedCar
        {
            get => _selectedCar;
            set
            {
                if (!ReferenceEquals(_selectedCar, value))
                {
                    _selectedCar = value;
                    NotifyPropertyChanged();
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
            ErrorMessage = string.Empty;

            try
            {
                var cars = await _apiClient.List();
                Cars.Clear();
                if (cars != null)
                {
                    foreach (var car in cars)
                    {
                        Cars.Add(car);
                    }
                }

                SelectedCar = Cars.FirstOrDefault();
            }
            catch (HttpRequestException exception)
            {
                ErrorMessage = $"Unable to load cars from http://localhost:5086/api/Cars. Make sure the WebAPI is running.\n\n{exception.Message}";
            }
            catch (TaskCanceledException)
            {
                ErrorMessage = "The request to load cars timed out. Make sure the WebAPI is running at http://localhost:5086.";
            }
            catch (JsonException exception)
            {
                ErrorMessage = $"The API returned an unreadable Cars response.\n\n{exception.Message}";
            }
        }

    }
}
