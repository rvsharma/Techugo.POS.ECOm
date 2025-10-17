using System;
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

namespace Techugo.POS.ECOm.Pages.Dashboard.PendingRequest
{
    /// <summary>
    /// Interaction logic for RejectOrderPopUp.xaml
    /// </summary>
    public partial class RejectOrderPopUp : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler CloseClicked;
        public event RoutedEventHandler AssignRiderClicked;
        private readonly ApiService _apiService;
        private ObservableCollection<ReasonVM> _riderList;
        public ObservableCollection<ReasonVM> ReasonList
        {
            get => _riderList;
            set
            {
                _riderList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReasonList)));
            }
        }
        private int _selectedReasonId;
        public int SelectedReasonId
        {
            get => _selectedReasonId;
            set
            {
                _selectedReasonId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedReasonId)));
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
        private string _selectReasonText;
        public string SelectReasonText
        {
            get => _selectReasonText;
            set
            {
                _selectReasonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectReasonText)));
            }
        }
        public RejectOrderPopUp(OrderDetailVM orderDetail)
        {
            InitializeComponent();
            DataContext = this;
            OrderDetails = orderDetail;
            SelectReasonText = "Please select a reason for rejecting order " + orderDetail.OrderNo;
            _apiService = ApiServiceFactory.Create();
            GetRasons(); ;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }
        private async void ConfirmReject_Click(object sender, RoutedEventArgs e)
        {
            var data = new { RiderID = SelectedReasonId, OrderIDs = new[] { OrderDetails.OrderID } };
            BaseResponse result = await _apiService.PutAsync<BaseResponse>("rider/assign-rider", data);
            if (result != null)
            {
                if (result.Success == true)
                {

                    AssignRiderClicked?.Invoke(this, new RoutedEventArgs());
                }
            }

        }

        private async void GetRasons()
        {
            List<ReasonVM> riderListResponse = await _apiService.GetAsync<List<ReasonVM>>("rider/rider-list?page=1&limit=10&filter=All");
            var list = riderListResponse ?? new List<ReasonVM>();

            ReasonList = new ObservableCollection<ReasonVM>(list);
        }
        private void RiderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RiderComboBox.SelectedValue is int riderId)
            {
                SelectedReasonId = riderId;
            }
        }
        public class ReasonVM
        {
            public int ReasonId { get; set; }
            public string Reason { get; set; }
        }
    }
}
