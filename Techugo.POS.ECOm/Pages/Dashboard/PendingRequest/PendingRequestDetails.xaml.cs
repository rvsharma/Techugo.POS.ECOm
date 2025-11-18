using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Services;

namespace Techugo.POS.ECOm.Pages.Dashboard.PendingRequest
{
    /// <summary>
    /// Interaction logic for PendingRequestDetails.xaml
    /// </summary>
    public partial class PendingRequestDetails : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler CloseClicked;
        public event RoutedEventHandler RequestRefresh;
        private Window _rejectOrderPopUpWindow;
        private readonly ApiService _apiService;

        private OrderDetailVM _orderDetails;
        public OrderDetailVM OrderDetails
        {
            get => _orderDetails;
            set
            {
                _orderDetails = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrderDetails)));
            }
        }
        public PendingRequestDetails(OrderDetailVM orderDetail)
        {
            InitializeComponent();
            OrderDetails = orderDetail;
            DataContext = OrderDetails;
            _apiService = ApiServiceFactory.Create();
        }

        private async void AcceptRow_Click(object sender, RoutedEventArgs e)
        {
            var order = DataContext as OrderDetailVM;
            if (order == null) return;

            try
            {
                var orderID = Convert.ToInt32(order.OrderID);
                var data = new { OrderIDs = new[] { orderID }, BranchStatus = "StoreAccepted" };

                BaseResponse result = await _apiService.PutAsync<BaseResponse>("order/update-order", data);
                if (result != null && result.Success)
                {
                    SnackbarService.Enqueue($"Accepted order {order.OrderNo}");
                }
                else
                {
                    SnackbarService.Enqueue(result?.Message ?? "Failed to accept order");
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Enqueue($"Failed to accept order: {ex.Message}");
            }
            finally
            {
                // Close the popup window that hosts this UserControl
                var wnd = Window.GetWindow(this);
                wnd?.Close();
            }
        }

        private async void RejectRow_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            var selectable = button?.DataContext as OrderDetailVM;
            
            if (selectable == null) return;
            var popUpData = new SelectableOrderDetail(selectable);
            popUpData.Item.OrderID = selectable.OrderID;
            popUpData.Item.OrderNo = selectable.OrderNo;

            var popup = new RejectOrderPopUp(popUpData);
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

        // Close button in XAML
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var wnd = Window.GetWindow(this);
            wnd?.Close();
        }

        private void CloseOrderDetailsPopUp(object sender, RoutedEventArgs e)
        {
            //if (_orderDetailsPopUpWindow != null)
            //{
            //    _orderDetailsPopUpWindow.Close();
            //    _orderDetailsPopUpWindow = null;
            //}
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
            if (_rejectOrderPopUpWindow != null)
            {
                _rejectOrderPopUpWindow.Close();
                _rejectOrderPopUpWindow = null;
            }
            var wnd = Window.GetWindow(this);
            wnd?.Close();

            // Notify parent to refresh its list
            RequestRefresh?.Invoke(this, new RoutedEventArgs());


            //LoadOrdersData();
            SnackbarService.Enqueue($"Order {orderItem?.OrderNo} Rejected Successfully");
        }
    }
}
