using System.Net.Http.Json;
using KooliProjekt.BlazorWasm.Api;

namespace KooliProjekt.BlazorWasm
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;

        public ApiClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IList<Car>> List()
        {
            using var response = await _client.GetAsync("api/Cars");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Car>>() ?? new List<Car>();
        }

        public async Task<OperationResultResponse<Car>> Get(int id)
        {
            using var response = await _client.GetAsync($"api/Cars/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OperationResultResponse<Car>>();
        }

        public async Task<OperationResultResponse<object>> Save(Car car)
        {
            using var response = await _client.PostAsJsonAsync("api/Cars", car);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OperationResultResponse<object>>();
        }
    }
}
