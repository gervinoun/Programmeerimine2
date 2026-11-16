using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace KooliProjekt.WindowsForms.Api
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IList<Car>> List(string numberPlate = null)
        {
            var requestUri = "api/Cars?page=1&pageSize=10";
            if (!string.IsNullOrWhiteSpace(numberPlate))
            {
                requestUri += $"&numberPlate={Uri.EscapeDataString(numberPlate)}";
            }

            return await _httpClient.GetFromJsonAsync<List<Car>>(requestUri);
        }

        public async Task<OperationResultResponse> Save(Car car)
        {
            using var response = await _httpClient.PostAsJsonAsync("api/Cars", car);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OperationResultResponse>();
        }

        public async Task<OperationResultResponse> Delete(int id)
        {
            using var response = await _httpClient.DeleteAsync($"api/Cars/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OperationResultResponse>();
        }
    }
}
