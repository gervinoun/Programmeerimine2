using System.Net.Http;
using System.Net.Http.Json;
using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            const string url = "http://localhost:5086/api/Cars?page=1&pageSize=10";

            try
            {
                using var client = new HttpClient();
                var cars = await client.GetFromJsonAsync<List<Car>>(url);
                dataGridView1.DataSource = cars;
            }
            catch (HttpRequestException)
            {
                MessageBox.Show(
                    "Unable to load cars. Make sure the WebAPI is running at http://localhost:5086.",
                    "Cars could not be loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show(
                    "The request to load cars timed out. Make sure the WebAPI is running at http://localhost:5086.",
                    "Cars could not be loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
