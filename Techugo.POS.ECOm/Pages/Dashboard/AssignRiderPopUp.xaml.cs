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
        public AssignRiderPopUp(OrderDetailVM orderDetail)
        {
            InitializeComponent();
            DataContext = this;
            _apiService = ApiServiceFactory.Create();
            GetRiders();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }
        private void AssignRider_Click(object sender, RoutedEventArgs e)
        {
            AssignRiderClicked?.Invoke(this, new RoutedEventArgs());
        }

        private async void GetRiders()
        {
            RiderListResponse riderListResponse = await _apiService.GetAsync<RiderListResponse>("rider/rider-list?page=1&limit=10&filter=All");
            var list = riderListResponse?.Data ?? new List<RiderVM>();
            //// Insert placeholder at the top
            //list.Insert(0, new RiderVM { RiderID = 0, Name = "Select va rider" });
            RiderList = new ObservableCollection<RiderVM>(list);
        }
    }
}
