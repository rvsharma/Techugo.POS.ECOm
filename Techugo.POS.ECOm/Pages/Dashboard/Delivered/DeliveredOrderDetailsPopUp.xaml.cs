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
            DeliveryChargeAmount.Text = _orderDetails.DeliveryCharge > 0 ? $"+₹{_orderDetails.DeliveryCharge}" : $"Free";
            DeliveryChargeAmount.Foreground = _orderDetails.DeliveryCharge == 0 ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 166, 62)) : new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 106, 114, 130));

            // Set Handling Charge text and color
            HandlingCharge.Text = _orderDetails.HandlingCharge > 0 ? $"+₹{_orderDetails.HandlingCharge}" : $"Free";
            HandlingCharge.Foreground = _orderDetails.HandlingCharge == 0 ? new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 166, 62)) : new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 106, 114, 130));

            // Handle Membership Display
            if (_orderDetails.Membership != null)
            {
                if (_orderDetails.IsMembershipPurchase)
                {
                    // Membership purchased in this order
                    MembershipPurchasedPanel.Visibility = Visibility.Visible;
                    MembershipExistingPanel.Visibility = Visibility.Collapsed;
                    
                    MembershipNamePurchased.Text = _orderDetails.Membership.MembershipName;
                    MembershipAmountPurchased.Text = $"+₹{_orderDetails.Membership.Amount}";
                }
                else
                {
                    if(_orderDetails.MembershipDiscount > 0)
                    {

                    // Membership was already existing (applied to this order)
                    MembershipPurchasedPanel.Visibility = Visibility.Collapsed;
                    MembershipExistingPanel.Visibility = Visibility.Visible;
                    MembershipDiscount.Text = $"-₹{_orderDetails.MembershipDiscount}";


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
                    OfferDiscount.Text = $"-₹{_orderDetails.OfferDiscount}";
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
                TotalSavedAmount.Text = $"-₹{_orderDetails.TotalDiscount}";
                TotalSavedAmount.Visibility = Visibility.Visible;
            }
            else
            {
                TotalSavedAmount.Text = "-₹0";
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
