using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class RefundManagementResponse : BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("data")]
        public List<RefundData> Data { get; set; }
    }
    public class RefundData
    {
        public decimal Amount { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public RefundOrderMaster OrderMaster { get; set; }
    }

    public class RefundOrderMaster
    {
        public string OrderNo { get; set; }
        public string OrderID { get; set; }
        public Customer Customer { get; set; }
        public OrderAddress OrderAddress { get; set; }
        public object OrderDetails { get; set; }
    }
}
