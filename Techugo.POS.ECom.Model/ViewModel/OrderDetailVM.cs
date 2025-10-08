using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model.ViewModel
{
    public class OrderDetailVM
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
        public string Address { get; set; }
        public string Rider { get; set; }
        public List<OrderTimeline> OrderTimelines { get; set; }
        public string Subscription { get; set; }
        public string BranchDeliverySlot { get; set; }
        public List<string> ItemImages { get; set; }
        public string Items { get; set; }
    }
}
