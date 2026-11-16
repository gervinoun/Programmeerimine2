using System;
using System.Net.Http;
using KooliProjekt.WindowsForms.Api;

namespace KooliProjekt.WindowsForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5086/")
            };
            Application.Run(new Form1(new ApiClient(httpClient)));
        }
    }
}