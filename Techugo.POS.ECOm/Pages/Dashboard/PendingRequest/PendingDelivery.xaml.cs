using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages.Dashboard
{
    public partial class PendingDelivery : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<SelectableOrderDetail> _orderData;
        private Window? _orderDetailsPopUpWindow;

        public ObservableCollection<SelectableOrderDetail> orderData
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

        public PendingDelivery()
        {
            InitializeComponent();
            DataContext = this;
            orderData = new ObservableCollection<SelectableOrderDetail>();
            _apiService = ApiServiceFactory.Create();
            LoadOrdersData();
        }

        private async void LoadOrdersData()
        {
            string formattedDate = "2025-10-08";
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
                        var address = data.AddressList.HouseNo.ToString() + ", "
                                        + data.AddressList.StreetNo.ToString() + ", "
                                        + data.AddressList.State.ToString() + ", "
                                        + data.AddressList.City.ToString() + ", "
                                        + data.AddressList.Pincode.ToString();

                        OrderDetailVM order = new OrderDetailVM
                        {
                            OrderID = data.OrderID,
                            OrderNo = data.OrderNo,
                            createdAt = data.createdAt,
                            ExpectedDeliveryDate = data.ExpectedDeliveryDate,
                            TotalAmount = data.TotalAmount,
                            PaidAmount = data.PaidAmount,
                            Status = data.Status,
                            Address = address,
                            ShortAddress = address.Length > 20 ? address.Substring(0, 20) + "..." : address,
                            PaymentMode = data.PaymentMode,
                            Subscription = data.Subscription,
                            OrderType = data.Subscription == null ? "One Time Order" : "Subscription Order",
                            OrderDetails = data.OrderDetails,
                            Customer = data.Customer,
                            BranchDeliverySlot = or.BranchDeliverySlot.StartTime + " - " + or.BranchDeliverySlot.EndTime,
                            ItemImages = or.ItemImages,
                            Items = data.OrderDetails.Count + " items(s)"
                        };

                        // wrap in selectable container
                        var selectable = new SelectableOrderDetail(order);
                        orderData.Add(selectable);
                    }
                }

                TotalOrdersText = $"Pending Delivey Orders ({orderResponse.TotalItems} orders)";
            }
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null) return;

            bool shouldSelect = cb.IsChecked == true;
            foreach (var s in orderData)
                s.IsSelected = shouldSelect;
        }

        private void OpenOrderDetailPoPUp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectable = button?.DataContext as SelectableOrderDetail;
            var orderItem = selectable?.Item;
            if (orderItem == null)
                return;

            var popup = new OrderDetailsPopUp(orderItem);
            popup.CloseClicked += CloseOrderDetailsPopUp;

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
