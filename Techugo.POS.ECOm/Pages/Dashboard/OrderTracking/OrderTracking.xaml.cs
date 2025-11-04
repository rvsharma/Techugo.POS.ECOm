using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using System.Windows;
using System.Windows.Controls;
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
            TrackingResponse orderResponse = await _apiService.GetAsync<TrackingResponse>("order/tracking-list?page=1&limit=10");
            if (orderResponse != null)
            {

                orderData.Clear();
                foreach (var or in orderResponse.Data)
                {
                    TrackingDetailResponse orderDetails = await _apiService.GetAsync<TrackingDetailResponse>("order/order-tracking/" + or.OrderID);

                    if (orderDetails.Data != null)
                    {
                        
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
