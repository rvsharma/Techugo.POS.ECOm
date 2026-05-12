using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;


namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for OrderDetailsPopUp.xaml
    /// </summary>
    public partial class DeliveredOrderDetailsPopUp : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler CloseClicked;

        private OrderDetailVM _orderDetails;
        public OrderDetailVM OrderDetails
        {
            get => _orderDetails;
            set
            {
                _orderDetails = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderDetails)));
                //UpdateMembershipAndOfferUI();
            }
        }
        public DeliveredOrderDetailsPopUp(OrderDetailVM orderDetail)
        {
            InitializeComponent();
            OrderDetails = orderDetail;
            DataContext = OrderDetails;
            UpdateMembershipAndOfferUI();
        }

        /// <summary>
        /// Updates the membership and offer UI elements based on order details
        /// </summary>
        private void UpdateMembershipAndOfferUI()
        {
            if (_orderDetails == null)
                return;

            // Set Delivery Charge text and color
            DeliveryChargeAmount.Text = _orderDetails.DeliveryCharge > 0 ? $"+Rs.{_orderDetails.DeliveryCharge}" : $"Free";
            DeliveryChargeAmount.Foreground = _orderDetails.DeliveryCharge == 0 ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 166, 62)) : new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 106, 114, 130));

            // Set Handling Charge text and color
            HandlingCharge.Text = _orderDetails.HandlingCharge > 0 ? $"+Rs.{_orderDetails.HandlingCharge}" : $"Free";
            HandlingCharge.Foreground = _orderDetails.HandlingCharge == 0 ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 166, 62)) : new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 106, 114, 130));

            GrossAmount.Text = $"Rs.{_orderDetails.TotalAmount}";
            GrossAmount.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 106, 114, 130));

            GrandTotalAmount.Text = $"Rs.{_orderDetails.TotalAmount + _orderDetails.DeliveryCharge - _orderDetails.OfferDiscount}";

            if(_orderDetails.RefundAmount > 0)
            {
                RefundAmount.Text = $"Rs.{_orderDetails.RefundAmount}";
                Refund.Visibility = Visibility.Visible;
            }

            PlatformChargeAmount.Text = _orderDetails.PlatformCharge != null && _orderDetails.PlatformCharge > 0 ? $"+Rs.{_orderDetails.PlatformCharge}" : $"Free";

            // Handle Membership Display
            if (_orderDetails.Membership != null)
            {
                if (_orderDetails.IsMembershipPurchase)
                {
                    // Membership purchased in this order
                    MembershipPurchasedPanel.Visibility = Visibility.Visible;
                    MembershipExistingPanel.Visibility = Visibility.Collapsed;
                    
                    MembershipNamePurchased.Text = _orderDetails.Membership.MembershipName;
                    MembershipAmountPurchased.Text = $"+Rs.{_orderDetails.Membership.Amount}";
                }
                else
                {
                    if(_orderDetails.MembershipDiscount > 0)
                    {

                    // Membership was already existing (applied to this order)
                    MembershipPurchasedPanel.Visibility = Visibility.Collapsed;
                    MembershipExistingPanel.Visibility = Visibility.Visible;
                    MembershipDiscount.Text = $"-Rs.{_orderDetails.MembershipDiscount}";


                    MembershipNameExisting.Text = _orderDetails.Membership.MembershipName;
                    }
                }
            }
            else
            {
                // No membership
                MembershipPurchasedPanel.Visibility = Visibility.Collapsed;
                MembershipExistingPanel.Visibility = Visibility.Collapsed;
            }

            // Handle Offer Display
            if (_orderDetails.Offer != null)
            {
                OfferPanel.Visibility = Visibility.Visible;
                OfferName.Text = _orderDetails.Offer.OfferName;
                
                if (_orderDetails.OfferDiscount > 0)
                {
                    OfferDiscount.Text = $"-Rs.{_orderDetails.OfferDiscount}";
                }
                else
                {
                    OfferDiscount.Text = "Offer Applied";
                }
            }
            else
            {
                OfferPanel.Visibility = Visibility.Collapsed;
            }

            // Handle Total Saved Display
            if (_orderDetails.TotalDiscount > 0)
            {
                TotalSaved.Visibility = Visibility.Visible;
                TotalSavedAmount.Text = $"-Rs.{_orderDetails.TotalDiscount}";
                TotalSavedAmount.Visibility = Visibility.Visible;
            }
            else
            {
                TotalSavedAmount.Text = "-Rs.0";
                TotalSavedAmount.Visibility = Visibility.Visible;
            }
            TotalAmount.Text =(_orderDetails.PaidAmount).ToString();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}
