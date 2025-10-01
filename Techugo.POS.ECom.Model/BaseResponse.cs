
using System.Text.Json.Serialization;


namespace Techugo.POS.ECom.Model
{
    public class BaseResponse
    {
        [JsonPropertyName("status_code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
