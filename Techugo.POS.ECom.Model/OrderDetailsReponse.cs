
using System.Text.Json.Serialization;

namespace Techugo.POS.ECom.Model
{
    public class OrderDetailsReponse: BaseResponse
    {
        [JsonPropertyName("data")]
        public OrderFullDetails Data { get; set; } = new OrderFullDetails();
    }

    public class OrderFullDetails
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; }
        public decimal DeliveryCharge { get; set; }
        public string PaymentMode { get; set; }
        public string BranchStatus { get; set; }
        public string RiderStatus { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public CustomerDetails Customer { get; set; }
        public AddressDetails AddressList { get; set; }
        public Rider Rider { get; set; }
        public List<OrderTimeline> OrderTimelines { get; set; }
        public string Subscription { get; set; }
        public BranchDeliverySlot BranchDeliverySlot { get; set; }
    }

    public class OrderDetail
    {
        public string ID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string UOM { get; set; }
        public decimal SPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Discount { get; set; }
        public ItemDetails Item { get; set; }
    }

    public class ItemDetails
    {
        public string ItemName { get; set; }
        public List<ItemImage> ItemImages { get; set; }
    }

    public class ItemImage
    {
        public string ImagePath { get; set; }
    }

    public class CustomerDetails
    {
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
    }

    public class AddressDetails
    {
        public string Name { get; set; }
        public string HouseNo { get; set; }
        public string StreetNo { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
    }

    public class OrderTimeline
    {
        public DateTime PlacedOn { get; set; }
        public DateTime? StoreAcceptedOn { get; set; }
        public DateTime? PackedOn { get; set; }
        public DateTime? RiderRequestedOn { get; set; }
        public DateTime? RiderAssignedOn { get; set; }
        public DateTime? RiderAcceptedOn { get; set; }
        public DateTime? PickedOn { get; set; }
        public DateTime? OutForDeliveryOn { get; set; }
        public DateTime? OnTheWayOn { get; set; }
        public DateTime? ReachedOn { get; set; }
        public DateTime? DeliveredOn { get; set; }
    }

    public class Rider
    {
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public bool? IsOnline { get; set; }
    }

}
