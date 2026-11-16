using System;
using System.Net.Http;
using System.Windows;
using KooliProjekt.WpfApplication.Api;

namespace KooliProjekt.WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5086/")
            };

            var viewModel = new MainWindowViewModel(new ApiClient(_httpClient), new DialogProvider());
            DataContext = viewModel;
            Loaded += async (_, _) => await viewModel.LoadCarsAsync();
            Closed += (_, _) => _httpClient.Dispose();
        }
    }
}