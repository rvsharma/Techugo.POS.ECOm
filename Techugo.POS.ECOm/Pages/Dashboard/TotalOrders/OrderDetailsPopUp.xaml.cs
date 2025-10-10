using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;


namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for OrderDetailsPopUp.xaml
    /// </summary>
    public partial class OrderDetailsPopUp : UserControl, INotifyPropertyChanged
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
        public OrderDetailsPopUp(OrderDetailVM orderDetail)
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
