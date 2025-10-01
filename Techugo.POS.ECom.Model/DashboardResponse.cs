using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class DashboardResponse : BaseResponse
    {

        [JsonPropertyName("data")]
        public OrderStatsData Data { get; set; }
    }

    public class OrderStatsData
    {
        [JsonPropertyName("totalOrders")]
        public int TotalOrders { get; set; }
        [JsonPropertyName("pendingRequest")]
        public int PendingRequest { get; set; }
        [JsonPropertyName("pickListOrder")]
        public int PickListOrder { get; set; }
        [JsonPropertyName("subscriptionOrders")]
        public int SubscriptionOrders { get; set; }
        [JsonPropertyName("assignRider")]
        public int AssignRider { get; set; }
        [JsonPropertyName("orderTracking")]
        public int OrderTracking { get; set; }
        [JsonPropertyName("returnRequests")]
        public int ReturnRequests { get; set; }
        [JsonPropertyName("cancelledOrders")]
        public int CancelledOrders { get; set; }
        [JsonPropertyName("deliveredOrders")]
        public int DeliveredOrders { get; set; }
    }
}
