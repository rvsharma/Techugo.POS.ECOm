using System;
using System.Collections.Generic;

namespace Techugo.POS.ECOm.Config
{
    // Central constants and lookup for status keys, display status and descriptions.
    public static class StatusConstants
    {
        // Keys (as they appear in appsettings.json)
        public const string Key_Delivered = "Delivered";
        public const string Key_Reached = "Reached";
        public const string Key_OnTheWay = "OnTheWay";
        public const string Key_OutForDelivery = "OutForDelivery";
        public const string Key_Picked = "Picked";
        public const string Key_RiderAccepted = "RiderAccepted";
        public const string Key_RiderRejected = "RiderRejected";
        public const string Key_Assigned = "Assigned";
        public const string Key_RiderRequested = "RiderRequest";
        public const string Key_Packed = "Packed";
        public const string Key_StoreRejected = "StoreRejected";
        public const string Key_StoreAccepted = "StoreAccepted";
        public const string Key_Placed = "Placed";

        // Display status values (what you want to show in UI)
        public const string Status_Delivered = "Delivered";
        public const string Status_Reached = "Reached";
        public const string Status_OnTheWay = "On The Way";
        public const string Status_OutForDelivery = "Out For Delivery";
        public const string Status_Picked = "Picked";
        public const string Status_RiderAccepted = "Rider Accepted";
        public const string Status_RiderRejected = "Rider Rejected";
        public const string Status_Assigned = "Assigned";
        public const string Status_RiderRequested = "Rider Requested";
        public const string Status_Packed = "Packed";
        public const string Status_StoreRejected = "Store Rejected";
        public const string Status_StoreAccepted = "Store Accepted";
        public const string Status_Placed = "Placed";

        // Descriptions
        public const string Desc_Delivered = "Delivered the Order. Delivered the Order Successfully!";
        public const string Desc_Reached = "reached the store. Collecting the order.";
        public const string Desc_OnTheWay = "has accepted your order. Coming to Pickup Orders.";
        public const string Desc_OutForDelivery = "Out for Delivery. Collected the order & out for the Delivery.";
        public const string Desc_Picked = "has been picked up order.";
        public const string Desc_RiderAccepted = "has accepted the order.";
        public const string Desc_RiderRejected = "has rejected the order.";
        public const string Desc_Assigned = "Order has been assigned to a rider.";
        public const string Desc_RiderRequested = "has been requested for the order.";
        public const string Desc_Packed = "Order has been packed.";
        public const string Desc_StoreRejected = "Order has been rejected by the store.";
        public const string Desc_StoreAccepted = "Order has been accepted by the store.";
        public const string Desc_Placed = "Order has been placed.";

        // Read-only lookup dictionary for convenience (case-insensitive keys)
        public static readonly IReadOnlyDictionary<string, (string Status, string Description)> Mappings
            = new Dictionary<string, (string Status, string Description)>(StringComparer.OrdinalIgnoreCase)
        {
            { Key_Delivered, (Status_Delivered, Desc_Delivered) },
            { Key_Reached, (Status_Reached, Desc_Reached) },
            { Key_OnTheWay, (Status_OnTheWay, Desc_OnTheWay) },
            { Key_OutForDelivery, (Status_OutForDelivery, Desc_OutForDelivery) },
            { Key_Picked, (Status_Picked, Desc_Picked) },
            { Key_RiderAccepted, (Status_RiderAccepted, Desc_RiderAccepted) },
            { Key_RiderRejected, (Status_RiderRejected, Desc_RiderRejected) },
            { Key_Assigned, (Status_Assigned, Desc_Assigned) },
            { Key_RiderRequested, (Status_RiderRequested, Desc_RiderRequested) },
            { Key_Packed, (Status_Packed, Desc_Packed) },
            { Key_StoreRejected, (Status_StoreRejected, Desc_StoreRejected) },
            { Key_StoreAccepted, (Status_StoreAccepted, Desc_StoreAccepted) },
            { Key_Placed, (Status_Placed, Desc_Placed) },
        };
    }
}