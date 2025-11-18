using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class OrdersResponse : BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public ObservableCollection<Order> Data { get; set; } = new ObservableCollection<Order>();
    }
    public class Order
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; }
        public string BranchStatus { get; set; }
        public string RiderStatus { get; set; }
        public string PaymentMode { get; set; }
        public string Subscription { get; set; }
        public BranchDeliverySlot BranchDeliverySlot { get; set; }
        public List<string> ItemImages { get; set; }
    }

    public class BranchDeliverySlot
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
