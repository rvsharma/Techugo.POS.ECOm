using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class DeliverdOrdersResponse : BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public ObservableCollection<DataItem> Data { get; set; } = new ObservableCollection<DataItem>();
    }
    public class DataItem
    {
        public string type { get; set; }
        public string society { get; set; }
        public string zone { get; set; }
        public List<DeliveredOrder> orders { get; set; }
    }

    public class DeliveredOrder
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public bool IsNextDayDelivery { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal? RefundAmount { get; set; }
        public string Status { get; set; }
        public string BranchStatus { get; set; }
        public string RiderStatus { get; set; }
        public string PaymentMode { get; set; }
        public string Subscription { get; set; }
        public Customer Customer { get; set; }
        public BranchDeliverySlot? BranchDeliverySlot { get; set; }
        public OrderAddress OrderAddress { get; set; }
        public List<string> ItemImages { get; set; }

    }

    public class Customer
    {
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
    }
    public class OrderAddress
    {
        //public int? OrderAddressID { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string HouseNo { get; set; }
        public string StreetNo { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Landmark { get; set; }
        public string TypeOfAddress { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public bool IsSocietyAddress { get; set; }
        public int? ZoneID { get; set; }
        public int? SocietyID { get; set; }
        //public string BranchZone { get; set; }
        public Society Society { get; set; }
    }

    public class Society
    {
        public int? SocietyID { get; set; }
        public string SocietyName { get; set; }
    }
}
