using System.Text.Json.Serialization;

namespace Techugo.POS.ECom.Model
{
    public class SalesAnalyticsResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        public SalesData Data { get; set; }
    }

    public class SalesData
    {
        [JsonPropertyName("weeklyComparison")]
        public List<WeeklyComparison> WeeklyComparison { get; set; }
        public Sales Sales { get; set; }
        public Orders2 Orders { get; set; }
        public AverageSale AverageSale { get; set; }


    }

    public class WeeklyComparison
    {
        [JsonPropertyName("day")]
        public string Day { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("lastWeek")]
        public decimal LastWeek { get; set; }
    }

    public class Sales
    {
        public decimal TotalSale { get; set; }
    }
    public class Orders2
    {
        public decimal TotalOrders { get; set; }
    }

    public class AverageSale
    {
        public string AvgSale { get; set; }
    }
}
