using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using KooliProjekt.WindowsForms.Api;
using Newtonsoft.Json;

namespace KooliProjekt.WindowsForms
{
    public partial class Form1 : Form, IMainView
    {
        private readonly IApiClient _apiClient;
        private MainViewPresenter _presenter;
        private bool _suppressSelectionChanged;

        public Form1(IApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<Car> DataSource
        {
            get => dataGridView1.DataSource as IList<Car>;
            set
            {
                var wasSuppressed = _suppressSelectionChanged;
                _suppressSelectionChanged = true;
                try
                {
                    dataGridView1.DataSource = value;
                }
                finally
                {
                    _suppressSelectionChanged = wasSuppressed;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Car SelectedItem
        {
            get => dataGridView1.SelectedRows.Count > 0
                ? dataGridView1.SelectedRows[0].DataBoundItem as Car
                : null;
            set
            {
                var wasSuppressed = _suppressSelectionChanged;
                _suppressSelectionChanged = true;
                try
                {
                    if (value == null)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = null;
                        return;
                    }

                    var row = dataGridView1.Rows
                        .Cast<DataGridViewRow>()
                        .FirstOrDefault(candidate => ReferenceEquals(candidate.DataBoundItem, value));

                    if (row != null && row.Cells.Count > 0)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = row.Cells[0];
                        row.Selected = true;
                    }
                }
                finally
                {
                    _suppressSelectionChanged = wasSuppressed;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Id
        {
            get => idTextBox.Text;
            set => idTextBox.Text = value ?? string.Empty;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NumberPlate
        {
            get => numberPlateTextBox.Text;
            set => numberPlateTextBox.Text = value ?? string.Empty;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TypeId
        {
            get => typeIdTextBox.Text;
            set => typeIdTextBox.Text = value ?? string.Empty;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Kmrate
        {
            get => kmRateTextBox.Text;
            set => kmRateTextBox.Text = value ?? string.Empty;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TimeRate
        {
            get => timeRateTextBox.Text;
            set => timeRateTextBox.Text = value ?? string.Empty;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAvailable
        {
            get => isAvailableCheckBox.Checked;
            set => isAvailableCheckBox.Checked = value;
        }

        public void SetPresenter(MainViewPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await _presenter.LoadData();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionChanged || _presenter == null)
            {
                return;
            }

            _presenter.SetSelection(SelectedItem);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            SelectedItem = null;
            _presenter.SetSelection(null);
            Kmrate = "0";
            TimeRate = "0";
            IsAvailable = true;
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

                await _presenter.LoadData(car.NumberPlate);
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

                await _presenter.LoadData();
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
            if (!string.IsNullOrWhiteSpace(Id) &&
                !int.TryParse(Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                validationMessage = "Car ID must be a whole number.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(NumberPlate))
            {
                validationMessage = "Number plate is required.";
                return false;
            }

            if (!int.TryParse(TypeId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var typeId) || typeId <= 0)
            {
                validationMessage = "Type ID must be a whole number greater than zero.";
                return false;
            }

            if (!decimal.TryParse(Kmrate, NumberStyles.Number, CultureInfo.CurrentCulture, out var kmRate) || kmRate < 0)
            {
                validationMessage = "Km rate must be a non-negative number.";
                return false;
            }

            if (!decimal.TryParse(TimeRate, NumberStyles.Number, CultureInfo.CurrentCulture, out var timeRate) || timeRate < 0)
            {
                validationMessage = "Time rate must be a non-negative number.";
                return false;
            }

            car = new Car
            {
                Id = id,
                NumberPlate = NumberPlate.Trim(),
                TypeId = typeId,
                Kmrate = kmRate,
                TimeRate = timeRate,
                IsAvailable = IsAvailable
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
            SelectedItem = null;
            _presenter.SetSelection(null);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Cars API", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
