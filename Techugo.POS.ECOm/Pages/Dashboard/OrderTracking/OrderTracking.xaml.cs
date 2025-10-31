using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages.Dashboard.OrderTracking
{
    /// <summary>
    /// Interaction logic for OrderTracking.xaml
    /// </summary>
    public partial class OrderTracking : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<OrderDetailVM> _orderData;
        private Window _orderDetailsPopUpWindow;
        public ObservableCollection<OrderDetailVM> orderData
        {
            get => _orderData;
            set
            {
                _orderData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(orderData)));
            }
        }

        private string _totalOrdersText;
        public string TotalOrdersText
        {
            get => _totalOrdersText;
            set
            {
                _totalOrdersText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalOrdersText)));
            }
        }
        public OrderTracking()
        {
            InitializeComponent();
            DataContext = this;
            orderData = new ObservableCollection<OrderDetailVM>();
            _apiService = ApiServiceFactory.Create();
            LoadOrdersData();
        }
        private async void LoadOrdersData()
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            OrdersResponse orderResponse = await _apiService.GetAsync<OrdersResponse>("order/tracking-list?page=1&limit=10");
            if (orderResponse != null)
            {

                orderData.Clear();
                foreach (var or in orderResponse.Data)
                {
                    OrderDetailsReponse orderDetails = await _apiService.GetAsync<OrderDetailsReponse>("order/order-detail/" + or.OrderID);

                    if (orderDetails.Data != null)
                    {
                        var data = orderDetails.Data;
                        var address = data.AddressList.HouseNo.ToString() + ", "
                                        + data.AddressList.StreetNo.ToString() + ", "
                                        + data.AddressList.State.ToString() + ", "
                                        + data.AddressList.City.ToString() + ", "
                                        + data.AddressList.Pincode.ToString();
                        OrderDetailVM order = new OrderDetailVM();
                        order.OrderID = data.OrderID;
                        order.OrderNo = data.OrderNo;
                        order.createdAt = data.createdAt;
                        order.ExpectedDeliveryDate = data.ExpectedDeliveryDate;
                        order.TotalAmount = data.TotalAmount;
                        order.PaidAmount = data.PaidAmount;
                        order.Status = data.Status;
                        order.Address = address;
                        order.PaymentMode = data.PaymentMode;
                        order.ShortAddress = address.Length > 20 ? address.Substring(0, 20) + "..." : address;
                        order.Subscription = data.Subscription;
                        order.OrderDetails = data.OrderDetails;
                        order.Customer = data.Customer;
                        order.BranchDeliverySlot = or.BranchDeliverySlot.StartTime + " - " + or.BranchDeliverySlot.EndTime;
                        order.ItemImages = or.ItemImages;
                        order.Items = data.OrderDetails.Count + " items(s)";
                        order.Status = data.Status;
                        orderData.Add(order);
                    }

                }

                TotalOrdersText = $"All Orders ({orderResponse.TotalItems} orders)";

            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}
