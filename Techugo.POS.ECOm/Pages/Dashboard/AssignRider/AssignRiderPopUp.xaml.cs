using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;


namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for AssignRiderPopUp.xaml
    /// </summary>
    public partial class AssignRiderPopUp : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler CloseClicked;
        public event RoutedEventHandler AssignRiderClicked;
        private readonly ApiService _apiService;
        private ObservableCollection<RiderVM> _riderList;
        public ObservableCollection<RiderVM> RiderList
        {
            get => _riderList;
            set
            {
                _riderList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RiderList)));
            }
        }
        private int _selectedRiderId;
        public int SelectedRiderId
        {
            get => _selectedRiderId;
            set
            {
                _selectedRiderId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRiderId)));
            }
        }
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
        public AssignRiderPopUp(OrderDetailVM orderDetail)
        {
            InitializeComponent();
            DataContext = this;
            OrderDetails = orderDetail;
            _apiService = ApiServiceFactory.Create();
            GetRiders();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }
        private async void AssignRider_Click(object sender, RoutedEventArgs e)
        {
            var data = new { RiderID = SelectedRiderId, OrderIDs = new[] { OrderDetails.OrderID } };
            BaseResponse result = await _apiService.PutAsync<BaseResponse>("rider/assign-rider", data);
            if (result != null)
            {
                if (result.Success == true)
                {

                    AssignRiderClicked?.Invoke(this, new RoutedEventArgs());
                }
            }
            
        }

        private async void GetRiders()
        {
            RiderListResponse riderListResponse = await _apiService.GetAsync<RiderListResponse>("rider/rider-list?page=1&limit=10&filter=All");
            var list = riderListResponse?.Data ?? new List<RiderVM>();

            RiderList = new ObservableCollection<RiderVM>(list);
        }
        private void RiderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RiderComboBox.SelectedValue is int riderId)
            {
                SelectedRiderId = riderId;
            }
        }
    }
}
