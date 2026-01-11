using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Pages.Dashboard.PendingRequest;
using Techugo.POS.ECOm.Services;

namespace Techugo.POS.ECOm.Pages.Dashboard
{
    public partial class PendingDelivery : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _api_service;
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<SelectableOrderDetail> _orderData = new();
        private Window? _orderDetailsPopUpWindow;
        private Window _rejectOrderPopUpWindow;

        public ObservableCollection<SelectableOrderDetail> orderData
        {
            get => _orderData;
            set
            {
                _orderData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(orderData)));
            }
        }

        private string _totalOrdersText = string.Empty;
        public string TotalOrdersText
        {
            get => _totalOrdersText;
            set
            {
                _totalOrdersText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalOrdersText)));
            }
        }

        private int _selectedCount;
        public int SelectedCount
        {
            get => _selectedCount;
            private set
            {
                if (_selectedCount == value) return;
                _selectedCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSelection)));
            }
        }

        public bool HasSelection => SelectedCount > 0;

        public PendingDelivery()
        {
            InitializeComponent();
            DataContext = this;
            orderData = new ObservableCollection<SelectableOrderDetail>();
            _api_service = ApiServiceFactory.Create();
            LoadOrdersData();
        }

        private async void LoadOrdersData()
        {
            //string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            string formattedDate = "2026-01-11";
            OrdersResponse orderResponse = await _api_service.GetAsync<OrdersResponse>("order/orders-list?OrderType=OneTime&page=1&limit=1000&status=PendingRequest&Date=" + formattedDate + "");
            if (orderResponse != null)
            {
                orderData.Clear();
                foreach (var or in orderResponse.Data)
                {
                    OrderDetailsReponse orderDetails = await _api_service.GetAsync<OrderDetailsReponse>("order/order-detail/" + or.OrderID);

                    if (orderDetails.Data != null)
                    {
                        var data = orderDetails.Data;
                        string address = string.Empty;
                        if (data.OrderAddress != null)
                        {
                            var parts = new List<string>();
                            if (!string.IsNullOrWhiteSpace(data.OrderAddress.HouseNo))
                                parts.Add(data.OrderAddress.HouseNo);
                            if (!string.IsNullOrWhiteSpace(data.OrderAddress.StreetNo))
                                parts.Add(data.OrderAddress.StreetNo);
                            if (!string.IsNullOrWhiteSpace(data.OrderAddress.State))
                                parts.Add(data.OrderAddress.State);
                            if (!string.IsNullOrWhiteSpace(data.OrderAddress.City))
                                parts.Add(data.OrderAddress.City);
                            if (!string.IsNullOrWhiteSpace(data.OrderAddress.Pincode))
                                parts.Add(data.OrderAddress.Pincode);

                            address = string.Join(", ", parts);
                        }

                        OrderDetailVM order = new OrderDetailVM
                        {
                            OrderID = data.OrderID,
                            OrderNo = data.OrderNo,
                            createdAt = data.createdAt,
                            ExpectedDeliveryDate = data.ExpectedDeliveryDate.HasValue
    ? data.ExpectedDeliveryDate.Value
    : null,
                            TotalAmount = data.PaidAmount,
                            PaidAmount = data.PaidAmount,
                            Status = data.Status,
                            Address = address,
                            ShortAddress = address.Length > 20 ? address.Substring(0, 20) + "..." : address,
                            PaymentMode = data.PaymentMode,
                            Subscription = data.Subscription,
                            OrderType = data.Subscription == null ? "One Time Order" : "Subscription Order",
                            OrderDetails = data.OrderDetails,
                            Customer = data.Customer,
                            BranchDeliverySlot = or.BranchDeliverySlot?.StartTime + " - " + or.BranchDeliverySlot?.EndTime,
                            ItemImages = or.ItemImages,
                            Items = data.OrderDetails.Count + " items(s)"
                        };

                        // wrap in selectable container
                        var selectable = new SelectableOrderDetail(order);
                        // subscribe to selection changes
                        selectable.PropertyChanged += Selectable_PropertyChanged;
                        orderData.Add(selectable);
                    }
                }

                TotalOrdersText = $"Pending Delivey Orders ({orderResponse.TotalItems} orders)";
                RecalculateSelectedCount();
            }
        }

        private void Selectable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectableOrderDetail.IsSelected))
                RecalculateSelectedCount();
        }

        private void RecalculateSelectedCount()
        {
            SelectedCount = orderData?.Count(x => x.IsSelected) ?? 0;
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null) return;

            bool shouldSelect = cb.IsChecked == true;
            foreach (var s in orderData)
                s.IsSelected = shouldSelect;

            RecalculateSelectedCount();
        }

        private async void AcceptSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrders = orderData.Where(x => x.IsSelected).Select(x => x.Item).ToList();
            if (!selectedOrders.Any())
                return;
            var orderIDs = selectedOrders.Select(o => Convert.ToInt32(o.OrderID)).ToArray();

            var data = new { OrderIDs = orderIDs, BranchStatus = "StoreAccepted" };
            BaseResponse result = await _api_service.PutAsync<BaseResponse>("order/update-order", data);
            if (result != null)
            {
                if (result.Success == true)
                {
                    SnackbarService.Enqueue($"{selectedOrders.Count} Orders Accepted");

                }
            }
            LoadOrdersData();
            // TODO: call your API to accept these orders. Placeholder for now:
            // MessageBox.Show($"Accepting {selectedOrders.Count} orders", "Accept Selected", MessageBoxButton.OK, MessageBoxImage.Information);

            // Optionally clear selection after action:
            foreach (var s in orderData.Where(x => x.IsSelected))
                s.IsSelected = false;
            RecalculateSelectedCount();
        }

        private async void RejectSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrders = orderData.Where(x => x.IsSelected).Select(x => x.Item).ToList();
            if (!selectedOrders.Any())
                return;
            var orderIDs = selectedOrders.Select(o => Convert.ToInt32(o.OrderID)).ToArray();

            var data = new { OrderIDs = orderIDs, BranchStatus = "StoreRejected" };
            BaseResponse result = await _api_service.PutAsync<BaseResponse>("order/update-order", data);
            if (result != null)
            {
                if (result.Success == true)
                {
                    SnackbarService.Enqueue($"{selectedOrders.Count} Orders Rejected");

                }
            }
            LoadOrdersData();
            // TODO: call your API to reject these orders. Placeholder for now:
            //MessageBox.Show($"Rejecting {selectedOrders.Count} orders", "Reject Selected", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Optionally clear selection after action:
            foreach (var s in orderData.Where(x => x.IsSelected))
                s.IsSelected = false;
            RecalculateSelectedCount();
        }

        private async void AcceptRow_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var selectable = btn?.DataContext as SelectableOrderDetail;
            var order = selectable?.Item;
            if (order == null) return;

            var orderID = Convert.ToInt32(order.OrderID);
            var data = new { OrderIDs = new[] { orderID }, BranchStatus = "StoreAccepted" };
            BaseResponse result = await _api_service.PutAsync<BaseResponse>("order/update-order", data);
            if (result != null)
            {
                if (result.Success == true)
                {
                    SnackbarService.Enqueue($"Accepted order {order.OrderNo}");

                }
            }
            LoadOrdersData();
            // TODO: replace placeholder with your API call to accept the order(s)
            //MessageBox.Show($"Accepted order {order.OrderNo}", "Accept", MessageBoxButton.OK, MessageBoxImage.Information);

            // optional: update UI/state after accepting
            selectable.IsSelected = false;
            RecalculateSelectedCount();
        }

        private async void RejectRow_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectable = button?.DataContext as SelectableOrderDetail;
            var order = selectable?.Item;
            if (order == null) return;

            var popup = new RejectOrderPopUp(selectable);
            popup.CloseClicked += CloseOrderDetailsPopUp;
            popup.PendingRequestClick += CloseRejectPopUp;

            _rejectOrderPopUpWindow = new Window
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
            _rejectOrderPopUpWindow.ShowDialog();
        }

        private void OpenOrderDetailPoPUp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectable = button?.DataContext as SelectableOrderDetail;
            var orderItem = selectable?.Item;
            if (orderItem == null)
                return;

            var popup = new PendingRequestDetails(orderItem);
            popup.CloseClicked += CloseOrderDetailsPopUp;

            // subscribe to refresh request from details popup so we can reload data
            popup.RequestRefresh += (s, ev) =>
            {
                // ensure UI actions run on UI thread
                Dispatcher.Invoke(() =>
                {
                    CloseOrderDetailsPopUp(this, new RoutedEventArgs());
                    LoadOrdersData();
                });
            };

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
            if (_rejectOrderPopUpWindow != null)
            {
                _rejectOrderPopUpWindow.Close();
                _rejectOrderPopUpWindow = null;
            }

        }

        private void CloseRejectPopUp(object sender, RoutedEventArgs e)
        {
            var button = sender as RejectOrderPopUp;
            var selectable = button?.DataContext as RejectOrderPopUp;
            var orderItem = selectable?.OrderDetails?.Item;

            // close reject popup if open
            if (_rejectOrderPopUpWindow != null)
            {
                _rejectOrderPopUpWindow.Close();
                _rejectOrderPopUpWindow = null;
            }

            // also close the order details popup (if open) as requested
            if (_orderDetailsPopUpWindow != null)
            {
                _orderDetailsPopUpWindow.Close();
                _orderDetailsPopUpWindow = null;
            }

            // refresh the list so the UI reflects the change
            LoadOrdersData();

            SnackbarService.Enqueue($"Order {orderItem?.OrderNo} Rejected Successfully");
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}