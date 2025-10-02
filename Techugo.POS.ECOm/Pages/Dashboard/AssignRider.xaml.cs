﻿using System;
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

namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for AssignRider.xaml
    /// </summary>
    public partial class AssignRider : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<OrderDetailVM> _orderData;
        private Window _assignRiderPopUpWindow;
        public ObservableCollection<OrderDetailVM> orderData
        {
            get => _orderData;
            set
            {
                _orderData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(orderData)));
            }
        }

        private string _assignRiderText;
        public string AssignRiderText
        {
            get => _assignRiderText;
            set
            {
                _assignRiderText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AssignRiderText)));
            }
        }
        public AssignRider()
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
            OrdersResponse orderResponse = await _apiService.GetAsync<OrdersResponse>("order/orders-list-by-zone?OrderType=OneTime&page=1&limit=10&status=AssignRider&Date=" + formattedDate + "");
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
                        order.OrderDetails = data.OrderDetails;
                        order.Customer = data.Customer;
                        order.BranchDeliverySlot = or.BranchDeliverySlot.StartTime + " - " + or.BranchDeliverySlot.EndTime;
                        order.ItemImages = or.ItemImages;
                        orderData.Add(order);
                    }

                }

                AssignRiderText = $"Assign Rider Orders ({orderResponse.TotalItems} orders)";

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

            var popup = new AssignRiderPopUp(orderItem);
            popup.CloseClicked += CloseOrderDetailsPopUp;
            popup.AssignRiderClicked += AssignRiderToOrder;

            // Option 1: Show as overlay in PageContent (replace current content)
            // SetPageContent(popup);

            // Option 2: Show as a dialog/modal (recommended for popups)
            // If you want a true modal, consider using a Window or a custom overlay.
            // Example:
            _assignRiderPopUpWindow = new Window
            {
                Content = popup,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Owner = Application.Current.MainWindow,
                Width = 800,
                Height = 420,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            _assignRiderPopUpWindow.ShowDialog();
        }

        private void CloseOrderDetailsPopUp(object sender, RoutedEventArgs e)
        {
            if (_assignRiderPopUpWindow != null)
            {
                _assignRiderPopUpWindow.Close();
                _assignRiderPopUpWindow = null;
            }
        }

        private void AssignRiderToOrder(object sender, RoutedEventArgs e)
        {
            if (_assignRiderPopUpWindow != null)
            {
                _assignRiderPopUpWindow.Close();
                _assignRiderPopUpWindow = null;
            }
        }
    }
}
