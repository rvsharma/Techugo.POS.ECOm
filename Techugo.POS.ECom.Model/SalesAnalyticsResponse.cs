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
        [JsonPropertyName("averageDailySales")]
        public int AverageDailySales { get; set; }
        [JsonPropertyName("averageDailySalesIncrease")]
        public string AverageDailySalesIncrease { get; set; }
        [JsonPropertyName("todayEarnings")]
        public int TodayEarnings { get; set; }
        [JsonPropertyName("todayEarningsIncrease")]
        public string TodayEarningsIncrease { get; set; }
        [JsonPropertyName("weeklyComparison")]
        public List<WeeklyComparison> WeeklyComparison { get; set; }
    }

    public class WeeklyComparison
    {
        [JsonPropertyName("day")]
        public string Day { get; set; }
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }

}
