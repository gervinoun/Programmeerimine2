using System.ComponentModel;
using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms
{
    public partial class Form1 : Form, IMainView
    {
        private MainViewPresenter _presenter;
        private bool _suppressSelectionChanged;

        public Form1()
        {
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
            _presenter.AddNew();
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            await _presenter.Save();
        }

        private async void deleteButton_Click(object sender, EventArgs e)
        {
            await _presenter.Delete();
        }

        public bool ConfirmDelete()
        {
            return MessageBox.Show(
                $"Are you sure you want to delete car {Id}?",
                "Confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        public void FocusNumberPlate()
        {
            numberPlateTextBox.Focus();
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Cars API", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
