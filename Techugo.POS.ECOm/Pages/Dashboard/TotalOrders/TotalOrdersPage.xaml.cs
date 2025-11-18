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
using System;
using System.Collections.Generic;

namespace Techugo.POS.ECOm.Pages
{
    public partial class TotalOrdersPage : UserControl, INotifyPropertyChanged
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

                        // Build address only if AddressList is not null. Add commas only between present parts.
                        string address = string.Empty;
                        if (data.AddressList != null)
                        {
                            var parts = new List<string>();
                            if (!string.IsNullOrWhiteSpace(data.AddressList.HouseNo))
                                parts.Add(data.AddressList.HouseNo);
                            if (!string.IsNullOrWhiteSpace(data.AddressList.StreetNo))
                                parts.Add(data.AddressList.StreetNo);
                            if (!string.IsNullOrWhiteSpace(data.AddressList.State))
                                parts.Add(data.AddressList.State);
                            if (!string.IsNullOrWhiteSpace(data.AddressList.City))
                                parts.Add(data.AddressList.City);
                            if (!string.IsNullOrWhiteSpace(data.AddressList.Pincode))
                                parts.Add(data.AddressList.Pincode);

                            address = string.Join(", ", parts);
                        }

                        OrderDetailVM order = new OrderDetailVM();
                        order.OrderID = data.OrderID;
                        order.OrderNo = data.OrderNo;
                        order.createdAt = data.createdAt;
                        order.ExpectedDeliveryDate = data.ExpectedDeliveryDate.HasValue ? data.ExpectedDeliveryDate.Value : null;
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
    }
}