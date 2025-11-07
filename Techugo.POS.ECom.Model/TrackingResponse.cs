
using System.Text.Json.Serialization;

namespace Techugo.POS.ECom.Model
{
    public class TrackingResponse: BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public List<TrackingItem> Data { get; set; } = new();
    }

    public class TrackingItem
    {
        [JsonPropertyName("OrderID")]
        public string OrderID { get; set; }

        [JsonPropertyName("OrderNo")]
        public string OrderNo { get; set; }

        [JsonPropertyName("Status")]
        public string Status { get; set; }

        [JsonPropertyName("BranchStatus")]
        public string BranchStatus { get; set; }

        [JsonPropertyName("RiderStatus")]
        public string RiderStatus { get; set; }

        public string Message { get; set; }

        [JsonPropertyName("Rider")]
        public RiderInfo Rider { get; set; }
    }

    public class RiderInfo
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("MobileNo")]
        public string MobileNo { get; set; }

        [JsonPropertyName("IsOnline")]
        public bool IsOnline { get; set; }
    }

    public class TrackingDetailResponse : BaseResponse
    {
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public TrackingDetail Data { get; set; }
    }

    public class TrackingDetail
    {
        [JsonPropertyName("OrderID")]
        public string OrderID { get; set; }

        [JsonPropertyName("ExpectedDeliveryDate")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [JsonPropertyName("OrderTimelines")]
        public List<TrackOrderTimeline> OrderTimelines { get; set; } = new();

        [JsonPropertyName("AddressList")]
        public AddressInfo AddressList { get; set; }

        [JsonPropertyName("Rider")]
        public TrackingRiderInfo Rider { get; set; }

        [JsonPropertyName("Customer")]
        public CustomerInfo Customer { get; set; }
    }

    public class TrackOrderTimeline
    {
        [JsonPropertyName("PaymentAuthorizedOn")]
        public DateTime? PaymentAuthorizedOn { get; set; }

        [JsonPropertyName("PaymentCapturedOn")]
        public DateTime? PaymentCapturedOn { get; set; }

        [JsonPropertyName("PaymentRefundCreatedOn")]
        public DateTime? PaymentRefundCreatedOn { get; set; }

        [JsonPropertyName("PaymentRefundProcessedOn")]
        public DateTime? PaymentRefundProcessedOn { get; set; }

        [JsonPropertyName("PaymentRefundFailedOn")]
        public DateTime? PaymentRefundFailedOn { get; set; }

        [JsonPropertyName("PaymentRefundPartialRefundOn")]
        public DateTime? PaymentRefundPartialRefundOn { get; set; }

        [JsonPropertyName("PaymentRefundOn")]
        public DateTime? PaymentRefundOn { get; set; }

        [JsonPropertyName("PaymentFailedOn")]
        public DateTime? PaymentFailedOn { get; set; }

        [JsonPropertyName("PaymentPaidOn")]
        public DateTime? PaymentPaidOn { get; set; }

        [JsonPropertyName("PendingOn")]
        public DateTime? PendingOn { get; set; }

        [JsonPropertyName("PlacedOn")]
        public DateTime? PlacedOn { get; set; }

        [JsonPropertyName("StoreAcceptedOn")]
        public DateTime? StoreAcceptedOn { get; set; }

        [JsonPropertyName("StoreRejectedOn")]
        public DateTime? StoreRejectedOn { get; set; }

        [JsonPropertyName("PackedOn")]
        public DateTime? PackedOn { get; set; }

        [JsonPropertyName("RiderRequestedOn")]
        public DateTime? RiderRequestedOn { get; set; }

        [JsonPropertyName("RiderAssignedOn")]
        public DateTime? RiderAssignedOn { get; set; }

        [JsonPropertyName("RiderRejectedOn")]
        public DateTime? RiderRejectedOn { get; set; }

        [JsonPropertyName("RiderAcceptedOn")]
        public DateTime? RiderAcceptedOn { get; set; }

        [JsonPropertyName("PickedOn")]
        public DateTime? PickedOn { get; set; }

        [JsonPropertyName("OutForDeliveryOn")]
        public DateTime? OutForDeliveryOn { get; set; }

        [JsonPropertyName("OnTheWayOn")]
        public DateTime? OnTheWayOn { get; set; }

        [JsonPropertyName("ReachedOn")]
        public DateTime? ReachedOn { get; set; }

        [JsonPropertyName("DeliveredOn")]
        public DateTime? DeliveredOn { get; set; }

        [JsonPropertyName("CancelledOn")]
        public DateTime? CancelledOn { get; set; }

        [JsonPropertyName("ReturnedOn")]
        public DateTime? ReturnedOn { get; set; }

        [JsonPropertyName("InvoiceDate")]
        public DateTime? InvoiceDate { get; set; }
    }

    public class AddressInfo
    {
        [JsonPropertyName("Pincode")]
        public string Pincode { get; set; }

        [JsonPropertyName("Lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("Long")]
        public double? Long { get; set; }

        [JsonPropertyName("Landmark")]
        public string Landmark { get; set; }
    }

    public class TrackingRiderInfo
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("MobileNo")]
        public string MobileNo { get; set; }

        [JsonPropertyName("Lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("Long")]
        public double? Long { get; set; }
    }

    public class CustomerInfo
    {
        [JsonPropertyName("CustomerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("MobileNo")]
        public string MobileNo { get; set; }
    }
}