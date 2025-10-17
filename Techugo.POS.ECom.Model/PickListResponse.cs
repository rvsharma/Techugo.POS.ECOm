using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class PickListResponse : BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("data")]
        public List<SocietyOrderGroup1> Data { get; set; }
    }

    public class SocietyOrderGroup1
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("society")]
        public string Society { get; set; }

        [JsonPropertyName("zone")]
        public string Zone { get; set; }

        [JsonPropertyName("orders")]
        public List<SocietyOrder1> Orders { get; set; }
    }

    public class SocietyOrder1
    {
        [JsonPropertyName("OrderID")]
        public string OrderID { get; set; }

        [JsonPropertyName("OrderNo")]
        public string OrderNo { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("ExpectedDeliveryDate")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [JsonPropertyName("TotalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("PaidAmount")]
        public decimal PaidAmount { get; set; }

        [JsonPropertyName("Status")]
        public string Status { get; set; }

        [JsonPropertyName("BranchStatus")]
        public string BranchStatus { get; set; }

        [JsonPropertyName("RiderStatus")]
        public string RiderStatus { get; set; }

        [JsonPropertyName("PaymentMode")]
        public string PaymentMode { get; set; }

        [JsonPropertyName("Subscription")]
        public string Subscription { get; set; }

        [JsonPropertyName("BranchDeliverySlot")]
        public BranchDeliverySlot BranchDeliverySlot { get; set; }

        [JsonPropertyName("AddressList")]
        public AddressList1 AddressList { get; set; }

        [JsonPropertyName("ItemImages")]
        public List<string> ItemImages { get; set; }
    }

    public class AddressList1
    {
        [JsonPropertyName("AddressID")]
        public int AddressID { get; set; }

        [JsonPropertyName("ZoneID")]
        public int? ZoneID { get; set; }

        [JsonPropertyName("IsSocietyAddress")]
        public bool IsSocietyAddress { get; set; }

        [JsonPropertyName("SocietyID")]
        public int? SocietyID { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("HouseNo")]
        public string HouseNo { get; set; }

        [JsonPropertyName("StreetNo")]
        public string StreetNo { get; set; }

        [JsonPropertyName("City")]
        public string City { get; set; }

        [JsonPropertyName("Pincode")]
        public string Pincode { get; set; }

        [JsonPropertyName("BranchZone")]
        public BranchZone BranchZone { get; set; }

        [JsonPropertyName("BranchSociety")]
        public BranchSociety1 BranchSociety { get; set; }
    }

    public class BranchSociety1
    {
        [JsonPropertyName("SocietyID")]
        public int SocietyID { get; set; }

        [JsonPropertyName("SocietyName")]
        public string SocietyName { get; set; }
    }
    public class BranchZone
    {
        [JsonPropertyName("SocietyID")]
        public int ZoneID { get; set; }

        [JsonPropertyName("SocietyName")]
        public string ZoneName { get; set; }
    }
}