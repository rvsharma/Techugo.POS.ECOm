using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;



namespace Techugo.POS.ECOm.Pages
{
    public partial class DashboardPage : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler TotalOrdersClicked;
        public event RoutedEventHandler PickListClicked;
        public event RoutedEventHandler AssignRiderClicked;
        public event RoutedEventHandler PendingDeliveryClicked;
        public event RoutedEventHandler DeliveredClicked;
        public event RoutedEventHandler RejectedClicked;
        public event RoutedEventHandler PartialReturnsClicked;
        public event RoutedEventHandler CarryForwardClicked;
        public event RoutedEventHandler OrderTrackingClicked;

        private readonly ApiService _apiService;

        public event PropertyChangedEventHandler PropertyChanged;

        private OrderStatsData _orderData;
        public OrderStatsData orderData
        {
            get => _orderData;
            set
            {
                _orderData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(orderData)));
            }
        }
        // Add events for other tiles

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = this;
            _apiService = ApiServiceFactory.Create();

            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                DashboardResponse data = await _apiService.GetAsync<DashboardResponse>("order/dashboard?Date=" + formattedDate + "");
                if (data != null)
                {
                    orderData = data.Data;
                }

            }
            catch (Exception ex) { }
            // TODO: Parse and display data in your dashboard UI
        }

        private void TotalOrders_Click(object sender, RoutedEventArgs e)
        {
            TotalOrdersClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void PickList_Click(object sender, RoutedEventArgs e)
        {
            PickListClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void AssignRider_Click(object sender, RoutedEventArgs e)
        {
            AssignRiderClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void PendingDelivery_Click(object sender, RoutedEventArgs e)
        {
            PendingDeliveryClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void Delivered_Click(object sender, RoutedEventArgs e)
        {
            DeliveredClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void Rejected_Click(object sender, RoutedEventArgs e)
        {
            RejectedClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void PartialReturns_Click(object sender, RoutedEventArgs e)
        {
            PartialReturnsClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void CarryForward_Click(object sender, RoutedEventArgs e)
        {
            CarryForwardClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void OrderTracking_Click(object sender, RoutedEventArgs e)
        {
            OrderTrackingClicked?.Invoke(this, new RoutedEventArgs());
        }
        // Add handlers for other tiles
        public void TriggerPickList()
        {
            // reuse same behavior as clicking the tile
            AssignRiderClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}