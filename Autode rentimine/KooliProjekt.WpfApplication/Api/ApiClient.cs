using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using KooliProjekt.WpfApplication.Api;
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

        public async Task<IList<Car>> List()
        {
            using var response = await _client.GetAsync("api/Cars");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Car>>(body) ?? new List<Car>();
        }
    }
}
