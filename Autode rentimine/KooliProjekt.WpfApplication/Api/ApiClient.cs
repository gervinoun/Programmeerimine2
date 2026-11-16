using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KooliProjekt.WpfApplication.Api
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;

        public ApiClient(HttpClient client)
        {
            _client = client ?? throw new System.ArgumentNullException(nameof(client));
        }

        public async Task<IList<Car>> List(string numberPlate = null)
        {
            var requestUri = "api/Cars";
            if (!string.IsNullOrWhiteSpace(numberPlate))
            {
                requestUri += $"?numberPlate={Uri.EscapeDataString(numberPlate)}";
            }

            using var response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Car>>(body) ?? new List<Car>();
        }

        public async Task<OperationResultResponse> Save(Car car)
        {
            var json = JsonConvert.SerializeObject(car);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _client.PostAsync("api/Cars", content);
            return await ReadOperationResultAsync(response);
        }

        public async Task<OperationResultResponse> Delete(int id)
        {
            using var response = await _client.DeleteAsync($"api/Cars/{id}");
            return await ReadOperationResultAsync(response);
        }

        private static async Task<OperationResultResponse> ReadOperationResultAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            OperationResultResponse result;

            try
            {
                result = JsonConvert.DeserializeObject<OperationResultResponse>(body);
            }
            catch (JsonException)
            {
                response.EnsureSuccessStatusCode();
                throw;
            }

            if (!response.IsSuccessStatusCode && result?.HasErrors != true)
            {
                response.EnsureSuccessStatusCode();
            }

            return result;
        }
    }
}
