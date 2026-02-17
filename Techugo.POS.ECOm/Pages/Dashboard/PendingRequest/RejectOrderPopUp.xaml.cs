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
using Techugo.POS.ECOm.Services;

namespace Techugo.POS.ECOm.Pages.Dashboard.PendingRequest
{
    /// <summary>
    /// Interaction logic for RejectOrderPopUp.xaml
    /// </summary>
    public partial class RejectOrderPopUp : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler CloseClicked;
        public event RoutedEventHandler PendingRequestClick;
        private readonly ApiService _apiService;
        private ObservableCollection<ReasonVM> _reasonList;
        public ObservableCollection<ReasonVM> ReasonList
        {
            get => _reasonList;
            set
            {
                _reasonList = value;
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
        private SelectableOrderDetail _orderDetails;
        public SelectableOrderDetail OrderDetails
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
        public RejectOrderPopUp(SelectableOrderDetail orderDetail)
        {
            InitializeComponent();
            DataContext = this;
            OrderDetails = orderDetail;
            SelectReasonText = "Please select a reason for rejecting order " + orderDetail.Item.OrderNo;
            _apiService = ApiServiceFactory.Create();
            GetRasons(); ;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }
        private async void ConfirmReject_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedReasonId == 0)
            {
                SnackbarService.Enqueue("Please select reason for rejection");
                return;
            }
            var orderID = Convert.ToInt32(OrderDetails.Item.OrderID);
            var data = new { ReasonID = SelectedReasonId, OrderIDs = new[] { orderID }, BranchStatus = "StoreRejected" };
            BaseResponse result = await _apiService.PutAsync<BaseResponse>("order/update-order", data);
            if (result != null)
            {
                if (result.Success == true)
                {

                    PendingRequestClick?.Invoke(this, new RoutedEventArgs());
                }
            }

        }

        private async void GetRasons()
        {
            ReasonResponse riderListResponse = await _apiService.GetAsync<ReasonResponse>("common/reason-list?Type=STORE_ORDER_CANCELLED");
            var list = riderListResponse ?? new ReasonResponse();

            ReasonList = new ObservableCollection<ReasonVM>(list.Data);
        }
        private void ReasonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReasonComboBox.SelectedValue is int reasonId)
            {
                SelectedReasonId = reasonId;
            }
        }
    }
}
