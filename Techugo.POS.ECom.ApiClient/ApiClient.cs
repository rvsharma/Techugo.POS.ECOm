using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.ApiClient
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public ApiService(IOptions<ApiSettings> apiSettings, string bearerToken)
        {
            _apiSettings = apiSettings.Value;
            _httpClient = new HttpClient { BaseAddress = new System.Uri(_apiSettings.BaseUrl) };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(responseString);
            if (result is null)
                throw new System.InvalidOperationException("Deserialization returned null.");
            return result;
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            // request.Headers.Add("Accept-Language", "en");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(responseString);
            if (result is null)
                throw new System.InvalidOperationException("Deserialization returned null.");
            return result;
        }

        public async Task<T> PutAsync<T>(string url, object data)
        {
            var result = default(T);
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            try
            {
                var response = await _httpClient.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                result = System.Text.Json.JsonSerializer.Deserialize<T>(responseString);
                if (result is null)
                    throw new System.InvalidOperationException("Deserialization returned null.");
                return result;
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode.HasValue &&
                                                      (httpEx.StatusCode.Value == HttpStatusCode.BadRequest ||
                                                       httpEx.StatusCode.Value == HttpStatusCode.NotFound))
            {
                // Try to get the response content from the exception's Data dictionary, if available
                if (httpEx.Data != null && httpEx.Data.Contains("ResponseContent"))
                {
                    var errorContentObj = httpEx.Data["ResponseContent"];
                    if (errorContentObj is string errorContent && !string.IsNullOrEmpty(errorContent))
                    {
                        result= System.Text.Json.JsonSerializer.Deserialize<T>(errorContent);
                        if (result != null)
                            return result;
                    }
                }
               return result;
            }
        }
    }
}