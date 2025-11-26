using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Pages.Dashboard;

namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for Delivered.xaml
    /// </summary>
    public partial class Delivered : UserControl, INotifyPropertyChanged
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
        private string _deliveredOrdersText;
        public string DeliverdOrdersText
        {
            get => _deliveredOrdersText;
            set
            {
                _deliveredOrdersText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeliverdOrdersText)));
            }
        }

        private ObservableCollection<DataItem> _groupedOrders;
        public ObservableCollection<DataItem> GroupedOrders
        {
            get => _groupedOrders;
            set
            {
                _groupedOrders = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupedOrders)));
            }
        }

        public Delivered()
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
            DeliverdOrdersResponse orderResponse = await _apiService.GetAsync<DeliverdOrdersResponse>("order/orders-list-by-zone?OrderType=OneTime&page=1&limit=1000&status=DeliveredOrders&Date=" + formattedDate + "");
            if (orderResponse != null)
            {

                var grouped = orderResponse.Data
           .GroupBy(g => g.society)
           .Select(grp => new DataItem
           {
               society = grp.Key,
               type = grp.First().type,
               zone = grp.First().zone,
               orders = grp.SelectMany(x => x.orders).ToList()
           })
           .ToList();
                GroupedOrders = new ObservableCollection<DataItem>(grouped);
                orderData.Clear();

                foreach (var groupedOrder in GroupedOrders)
                {
                    foreach (var o in groupedOrder.orders)
                    {
                        OrderDetailsReponse orderDetails = await _apiService.GetAsync<OrderDetailsReponse>("order/order-detail/" + o.OrderID);

                        if (orderDetails.Data != null)
                        {
                            var data = orderDetails.Data;
                            string address = string.Empty;
                            if (o.OrderAddress != null)
                            {
                                var parts = new List<string>();
                                if (!string.IsNullOrWhiteSpace(o.OrderAddress.HouseNo))
                                    parts.Add(o.OrderAddress.HouseNo);
                                if (!string.IsNullOrWhiteSpace(o.OrderAddress.StreetNo))
                                    parts.Add(o.OrderAddress.StreetNo);
                                if (!string.IsNullOrWhiteSpace(o.OrderAddress.State))
                                    parts.Add(o.OrderAddress.State);
                                if (!string.IsNullOrWhiteSpace(o.OrderAddress.City))
                                    parts.Add(o.OrderAddress.City);
                                if (!string.IsNullOrWhiteSpace(o.OrderAddress.Pincode))
                                    parts.Add(o.OrderAddress.Pincode);

                                address = string.Join(", ", parts);
                            }
                            OrderDetailVM order = new OrderDetailVM();
                            order.OrderID = data.OrderID;
                            order.OrderNo = data.OrderNo;
                            order.createdAt = data.createdAt;
                            order.ExpectedDeliveryDate = data.ExpectedDeliveryDate.HasValue
    ? data.ExpectedDeliveryDate.Value
    : null;
                            order.TotalAmount = data.PaidAmount;
                            order.PaidAmount = data.PaidAmount;
                            order.Status = data.Status;
                            order.Address = address;

                            order.ShortAddress = address.Length > 20 ? address.Substring(0, 20) + "..." : address;
                            order.PaymentMode = data.PaymentMode;
                            order.Subscription = data.Subscription;
                            order.OrderDetails = data.OrderDetails;
                            order.Customer = data.Customer;
                            order.BranchDeliverySlot = o.BranchDeliverySlot?.StartTime + " - " + o.BranchDeliverySlot?.EndTime;
                            order.ItemImages = o.ItemImages;
                            order.Status = o.Status;
                            order.Items = data.OrderDetails.Count + " items(s)";
                            orderData.Add(order);
                        }
                    }

                }







        //        foreach (var or in orderResponse.Data)
        //        {
        //            if (or.OrderID != null)
        //            {
        //                OrderDetailsReponse orderDetails = await _apiService.GetAsync<OrderDetailsReponse>("order/order-detail/" + or.OrderID);

        //                if (orderDetails.Data != null)
        //                {
        //                    var data = orderDetails.Data;
        //                    OrderDetailVM order = new OrderDetailVM();
        //                    order.OrderID = data.OrderID;
        //                    order.OrderNo = data.OrderNo;
        //                    order.createdAt = data.createdAt;
        //                    order.ExpectedDeliveryDate = data.ExpectedDeliveryDate.HasValue
        //? data.ExpectedDeliveryDate.Value
        //: null;
        //                    order.TotalAmount = data.TotalAmount;
        //                    order.PaidAmount = data.PaidAmount;
        //                    order.Status = data.Status;
        //                    string address = string.Empty;
        //                    if (data.AddressList != null)
        //                    {
        //                        var parts = new List<string>();
        //                        if (!string.IsNullOrWhiteSpace(data.AddressList.HouseNo))
        //                            parts.Add(data.AddressList.HouseNo);
        //                        if (!string.IsNullOrWhiteSpace(data.AddressList.StreetNo))
        //                            parts.Add(data.AddressList.StreetNo);
        //                        if (!string.IsNullOrWhiteSpace(data.AddressList.State))
        //                            parts.Add(data.AddressList.State);
        //                        if (!string.IsNullOrWhiteSpace(data.AddressList.City))
        //                            parts.Add(data.AddressList.City);
        //                        if (!string.IsNullOrWhiteSpace(data.AddressList.Pincode))
        //                            parts.Add(data.AddressList.Pincode);

        //                        address = string.Join(", ", parts);
        //                    }
        //                    order.Address = address;
        //                    order.PaymentMode = data.PaymentMode;
        //                    order.Subscription = data.Subscription;
        //                    order.OrderDetails = data.OrderDetails;
        //                    order.Customer = data.Customer;
        //                    order.BranchDeliverySlot = or.BranchDeliverySlot?.StartTime + " - " + or.BranchDeliverySlot?.EndTime;
        //                    order.ItemImages = or.ItemImages;
        //                    order.Status = or.Status;
        //                    order.Items = data.OrderDetails.Count + " items(s)";
        //                    orderData.Add(order);
        //                }
        //            }
        //        }
                DeliverdOrdersText = $"Delivered Orders ({orderResponse?.TotalItems} orders)";
            }
           
        }
        private void OpenOrderDetailPoPUp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderItem = button?.DataContext as OrderDetailVM;
            if (orderItem == null)
                return;

            var popup = new OrderDetailsPopUp(orderItem);
            popup.CloseClicked += CloseOrderDetailsPopUp;

            // Option 1: Show as overlay in PageContent (replace current content)
            // SetPageContent(popup);

            // Option 2: Show as a dialog/modal (recommended for popups)
            // If you want a true modal, consider using a Window or a custom overlay.
            // Example:
            _orderDetailsPopUpWindow = new Window
            {
                Content = popup,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Owner = Application.Current.MainWindow,
                Width = SystemParameters.PrimaryScreenWidth,
                Height = SystemParameters.PrimaryScreenHeight,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            _orderDetailsPopUpWindow.ShowDialog();
        }

        private void CloseOrderDetailsPopUp(object sender, RoutedEventArgs e)
        {
            if (_orderDetailsPopUpWindow != null)
            {
                _orderDetailsPopUpWindow.Close();
                _orderDetailsPopUpWindow = null;
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}
