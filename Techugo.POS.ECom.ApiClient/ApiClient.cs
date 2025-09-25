using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Techugo.POS.ECOm.ApiClient
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(string baseUrl, string username, string password)
        {
            _httpClient = new HttpClient { BaseAddress = new System.Uri(baseUrl) };
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<string> GetAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}