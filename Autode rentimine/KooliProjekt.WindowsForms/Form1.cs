using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms
{
    public partial class Form1 : Form
    {
        private readonly IApiClient _apiClient;

        public Form1(IApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData(string numberPlateToSelect = null)
        {
            try
            {
                var cars = await _apiClient.List();

                Car carToSelect = null;
                if (!string.IsNullOrWhiteSpace(numberPlateToSelect))
                {
                    carToSelect = cars.FirstOrDefault(car =>
                        string.Equals(car.NumberPlate, numberPlateToSelect, StringComparison.OrdinalIgnoreCase));

                    if (carToSelect == null)
                    {
                        var matchingCars = await _apiClient.List(numberPlateToSelect);
                        carToSelect = matchingCars.FirstOrDefault(car =>
                            string.Equals(car.NumberPlate, numberPlateToSelect, StringComparison.OrdinalIgnoreCase));

                        if (carToSelect != null)
                        {
                            cars = cars.Append(carToSelect).ToList();
                        }
                    }
                }

                dataGridView1.DataSource = cars;

                if (carToSelect != null)
                {
                    var row = dataGridView1.Rows
                        .Cast<DataGridViewRow>()
                        .FirstOrDefault(candidate => ReferenceEquals(candidate.DataBoundItem, carToSelect));

                    if (row != null && row.Cells.Count > 0)
                    {
                        dataGridView1.CurrentCell = row.Cells[0];
                        row.Selected = true;
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                ShowError($"Unable to load cars. Make sure the WebAPI is running at http://localhost:5086.\n\n{exception.Message}");
            }
            catch (TaskCanceledException)
            {
                ShowError("The request to load cars timed out. Make sure the WebAPI is running at http://localhost:5086.");
            }
            catch (JsonException exception)
            {
                ShowError($"The API returned an unreadable Cars response.\n\n{exception.Message}");
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is not Car car)
            {
                return;
            }

            idTextBox.Text = car.Id.ToString(CultureInfo.InvariantCulture);
            numberPlateTextBox.Text = car.NumberPlate;
            typeIdTextBox.Text = car.TypeId.ToString(CultureInfo.InvariantCulture);
            kmRateTextBox.Text = car.Kmrate.ToString(CultureInfo.CurrentCulture);
            timeRateTextBox.Text = car.TimeRate.ToString(CultureInfo.CurrentCulture);
            isAvailableCheckBox.Checked = car.IsAvailable;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
            idTextBox.Clear();
            numberPlateTextBox.Clear();
            typeIdTextBox.Clear();
            kmRateTextBox.Text = "0";
            timeRateTextBox.Text = "0";
            isAvailableCheckBox.Checked = true;
            numberPlateTextBox.Focus();
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            if (!TryBuildCar(out var car, out var validationMessage))
            {
                ShowError(validationMessage);
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
        }

        private async void deleteButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(idTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) || id <= 0)
            {
                ShowError("Select a saved car to delete.");
                return;
            }

            if (MessageBox.Show(
                    $"Are you sure you want to delete car {id}?",
                    "Confirm deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
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
                ClearCarFields();
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
        }

        private bool TryBuildCar(out Car car, out string validationMessage)
        {
            car = null;
            validationMessage = null;

            var id = 0;
            if (!string.IsNullOrWhiteSpace(idTextBox.Text) &&
                !int.TryParse(idTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                validationMessage = "Car ID must be a whole number.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(numberPlateTextBox.Text))
            {
                validationMessage = "Number plate is required.";
                return false;
            }

            if (!int.TryParse(typeIdTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var typeId) || typeId <= 0)
            {
                validationMessage = "Type ID must be a whole number greater than zero.";
                return false;
            }

            if (!decimal.TryParse(kmRateTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var kmRate) || kmRate < 0)
            {
                validationMessage = "Km rate must be a non-negative number.";
                return false;
            }

            if (!decimal.TryParse(timeRateTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var timeRate) || timeRate < 0)
            {
                validationMessage = "Time rate must be a non-negative number.";
                return false;
            }

            car = new Car
            {
                Id = id,
                NumberPlate = numberPlateTextBox.Text.Trim(),
                TypeId = typeId,
                Kmrate = kmRate,
                TimeRate = timeRate,
                IsAvailable = isAvailableCheckBox.Checked
            };
            return true;
        }

        private bool ShowApiErrors(OperationResultResponse result)
        {
            if (result == null)
            {
                ShowError("The API returned an empty response.");
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

            ShowError(messages.Count > 0 ? string.Join(Environment.NewLine, messages) : "The API rejected the request.");
            return true;
        }

        private void ClearCarFields()
        {
            idTextBox.Clear();
            numberPlateTextBox.Clear();
            typeIdTextBox.Clear();
            kmRateTextBox.Clear();
            timeRateTextBox.Clear();
            isAvailableCheckBox.Checked = false;
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Cars API", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
