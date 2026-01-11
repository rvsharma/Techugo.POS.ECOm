using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Services;

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
        private readonly WeightViewModel _weightVm = new WeightViewModel();
        public WeightViewModel WeightVM => _weightVm;

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

        private string _updatedTime;
        public string UpdatedTime
        {
            get => _updatedTime;
            set
            {
                _updatedTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdatedTime)));
            }
        }

        public DashboardPage()
        {
            InitializeComponent();
            // keep DataContext as the page so existing bindings (orderData, UpdatedTime, etc.) keep working
            DataContext = this;
            _apiService = ApiServiceFactory.Create();
            _ = LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                DashboardResponse data = await _apiService.GetAsync<DashboardResponse>("order/dashboard?Date=" + formattedDate + "");
                if (data != null)
                {
                    orderData = data.Data;
                    UpdatedTime = "Last updated: " + DateTime.Now.ToString("h:mm:ss tt");
                }
            }
            catch (Exception ex) { /* consider logging ex */ }
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

        public void TriggerPickList()
        {
            AssignRiderClicked?.Invoke(this, new RoutedEventArgs());
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await ApiHelper.RunWithLoader(async () =>
            {
                await Task.Delay(500).ConfigureAwait(false); // 2 second delay
                await LoadDashboardData().ConfigureAwait(false);
            }, "Refreshing dashboard...");
        }
    }
}