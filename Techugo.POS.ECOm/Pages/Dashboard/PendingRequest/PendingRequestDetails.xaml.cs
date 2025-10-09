using System;
using System.Collections.Generic;
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

namespace Techugo.POS.ECOm.Pages.Dashboard.PendingRequest
{
    /// <summary>
    /// Interaction logic for PendingRequestDetails.xaml
    /// </summary>
    public partial class PendingRequestDetails : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler CloseClicked;

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
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}
