using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class NotificationsResponse: BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public List<Notification> Data { get; set; }
    }
    public class Notification
    {
        public string NotificationID { get; set; }
        public string BranchID { get; set; }
        public string OrderID { get; set; }
        public string SettlementID { get; set; }   // nullable in JSON, so keep string or make it nullable type
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public bool IsRead { get; set; }
        public string IsActive { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        public OrderMaster OrderMaster { get; set; }
    }

    public class OrderMaster
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
    }

}
