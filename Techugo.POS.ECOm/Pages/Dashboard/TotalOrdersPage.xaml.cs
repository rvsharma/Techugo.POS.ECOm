using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages
{
    public partial class TotalOrdersPage : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<OrderDetailVM> _orderData;
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
        public TotalOrdersPage()
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
            OrdersResponse orderResponse = await _apiService.GetAsync<OrdersResponse>("order/orders-list?OrderType=OneTime&page=1&limit=10&status=TotalOrders&Date=" + formattedDate + "");
            if (orderResponse != null)
            {
                
                orderData.Clear();
                foreach (var or in orderResponse.Data)
                {
                    OrderDetailsReponse orderDetails = await _apiService.GetAsync<OrderDetailsReponse>("order/order-detail/" + or.OrderID);
                    
                    if (orderDetails.Data != null)
                    {
                        var data = orderDetails.Data;
                        OrderDetailVM order = new OrderDetailVM();
                        order.OrderID = data.OrderID;
                        order.OrderNo = data.OrderNo;
                        order.createdAt = data.createdAt;
                        order.ExpectedDeliveryDate = data.ExpectedDeliveryDate;
                        order.TotalAmount = data.TotalAmount;
                        order.PaidAmount = data.PaidAmount;
                        order.Status = data.Status;
                        order.Address = data.AddressList.HouseNo.ToString() + ", " 
                                        + data.AddressList.StreetNo.ToString() + ", " 
                                        + data.AddressList.State.ToString() + ", " 
                                        + data.AddressList.City.ToString() + ", " 
                                        + data.AddressList.Pincode.ToString();
                        order.PaymentMode = data.PaymentMode;
                        order.Subscription = data.Subscription;
                        order.OrderDetails  = data.OrderDetails;
                        order.Customer = data.Customer;
                        order.BranchDeliverySlot =or.BranchDeliverySlot.StartTime + " - " + or.BranchDeliverySlot.EndTime;
                        order.ItemImages = or.ItemImages;
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