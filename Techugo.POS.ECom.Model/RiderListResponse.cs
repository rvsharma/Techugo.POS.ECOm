using System.Text.Json.Serialization;

namespace Techugo.POS.ECom.Model
{
    public class RiderListResponse : BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public List<RiderVM> Data { get; set; }
    }

    public class RiderVM
    {
        [JsonPropertyName("RiderID")]
        public int RiderID { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("CountryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("MobileNo")]
        public string MobileNo { get; set; }

        [JsonPropertyName("EmailID")]
        public string EmailID { get; set; }

        [JsonPropertyName("Address")]
        public string Address { get; set; }

        [JsonPropertyName("Lat")]
        public double Lat { get; set; }

        [JsonPropertyName("Long")]
        public double Long { get; set; }

        [JsonPropertyName("RiderType")]
        public string RiderType { get; set; }

        [JsonPropertyName("IsOnline")]
        public bool IsOnline { get; set; }
    }
}
