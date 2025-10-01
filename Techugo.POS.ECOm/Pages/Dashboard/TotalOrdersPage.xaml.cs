using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages
{
    public partial class TotalOrdersPage : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Order> _orderData;
        public ObservableCollection<Order> orderData
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
            orderData = new ObservableCollection<Order>();
            // Get ApiSettings from DI container
            var apiSettingsOptions = App.ServiceProvider?.GetService(typeof(IOptions<ApiSettings>)) is IOptions<ApiSettings> options ? options : null;
            if (apiSettingsOptions == null)
            {
                throw new System.Exception("ApiSettings not configured.");
            }

            // Use the token stored in TokenService
            _apiService = new ApiService(apiSettingsOptions, TokenService.BearerToken);
            LoadOrdersData();
        }

        private async void LoadOrdersData()
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            OrdersResponse data = await _apiService.GetAsync<OrdersResponse>("order/orders-list?OrderType=OneTime&page=1&limit=10&status=TotalOrders&Date=" + formattedDate + "");
            if (data != null)
            {
                
                orderData.Clear();
                foreach (var order in data.Data) // assuming your API returns a list
                {
                    orderData.Add(order);
                }
                // Update TotalOrdersText on the UI thread
               
                    TotalOrdersText = $"All Orders ({data.TotalItems} orders)";
               
                //orderData = data.Data;
            }
            // TODO: Parse and display data in your dashboard UI
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}