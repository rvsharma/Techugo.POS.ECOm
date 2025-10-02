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

namespace Techugo.POS.ECOm.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for AssignRider.xaml
    /// </summary>
    public partial class AssignRider : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<OrderDetailVM> _orderData;
        private Window _assignRiderPopUpWindow;
        private ObservableCollection<SocietyOrderGroup> _groupedOrders;
        public ObservableCollection<SocietyOrderGroup> GroupedOrders
        {
            get => _groupedOrders;
            set
            {
                _groupedOrders = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupedOrders)));
            }
        }
        public ObservableCollection<OrderDetailVM> orderData
        {
            get => _orderData;
            set
            {
                _orderData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(orderData)));
            }
        }

        private string _assignRiderText;
        public string AssignRiderText
        {
            get => _assignRiderText;
            set
            {
                _assignRiderText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AssignRiderText)));
            }
        }
        public AssignRider()
        {
            InitializeComponent();
            DataContext = this;
            GroupedOrders = new ObservableCollection<SocietyOrderGroup>();
            orderData = new ObservableCollection<OrderDetailVM>();
            _apiService = ApiServiceFactory.Create();
            LoadOrdersData();
        }
        private async void LoadOrdersData()
        {
            //string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            string formattedDate = "2025-09-30";

            AssignRiderOrdersResponse assignRiderOrdersResponse = await _apiService.GetAsync<AssignRiderOrdersResponse>("order/orders-list-by-zone?OrderType=OneTime&page=1&limit=10&status=AssignRider&Date=" + formattedDate + "");
            if (assignRiderOrdersResponse != null)
            {

                var grouped = assignRiderOrdersResponse.Data
             .GroupBy(g => g.Society)
             .Select(grp => new SocietyOrderGroup
             {
                 Society = grp.Key,
                 Type = grp.First().Type,
                 Zone = grp.First().Zone,
                 Orders = grp.SelectMany(x => x.Orders).ToList()
             })
             .ToList();

                GroupedOrders = new ObservableCollection<SocietyOrderGroup>(grouped);
                orderData.Clear();
                foreach (var groupedOrder in GroupedOrders)
                {
                    foreach (var o in groupedOrder.Orders)
                    {
                        OrderDetailVM order = new OrderDetailVM();
                        order.OrderID = o.OrderID;
                        order.OrderNo = o.OrderNo;
                        order.createdAt = o.CreatedAt;
                        order.ExpectedDeliveryDate = o.ExpectedDeliveryDate;
                        order.TotalAmount = o.TotalAmount;
                        order.PaidAmount = o.PaidAmount;
                        order.Status = o.Status;
                        order.Address = o.AddressList.HouseNo.ToString() + ", "
                                        + o.AddressList.StreetNo.ToString() + ", "
                                        + o.AddressList.City.ToString() + ", "
                                        + o.AddressList.Pincode.ToString();
                        order.PaymentMode = o.PaymentMode;
                        order.Subscription = o.Subscription;
                        order.Customer = new CustomerDetails { CustomerName = o.AddressList.Name, MobileNo = "34234" };
                        order.BranchDeliverySlot = o.BranchDeliverySlot.StartTime + " - " + o.BranchDeliverySlot.EndTime;
                        order.ItemImages = o.ItemImages;
                        orderData.Add(order);
                    }
                    
                }

                AssignRiderText = $"Assign Rider Orders ({assignRiderOrdersResponse.TotalItems} orders)";

            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
        private void OpenOrderDetailPoPUp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderItem = button?.DataContext as OrderDetailVM;
            if (orderItem == null)
                return;

            var popup = new AssignRiderPopUp(orderItem);
            popup.CloseClicked += CloseOrderDetailsPopUp;
            popup.AssignRiderClicked += AssignRiderToOrder;

            // Option 1: Show as overlay in PageContent (replace current content)
            // SetPageContent(popup);

            // Option 2: Show as a dialog/modal (recommended for popups)
            // If you want a true modal, consider using a Window or a custom overlay.
            // Example:
            _assignRiderPopUpWindow = new Window
            {
                Content = popup,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Owner = Application.Current.MainWindow,
                Width = 420,
                Height = 230,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            _assignRiderPopUpWindow.ShowDialog();
        }

        private void CloseOrderDetailsPopUp(object sender, RoutedEventArgs e)
        {
            if (_assignRiderPopUpWindow != null)
            {
                _assignRiderPopUpWindow.Close();
                _assignRiderPopUpWindow = null;
            }
        }

        private void AssignRiderToOrder(object sender, RoutedEventArgs e)
        {
            if (_assignRiderPopUpWindow != null)
            {
                _assignRiderPopUpWindow.Close();
                _assignRiderPopUpWindow = null;
            }
            ShowSuccessSnackbar("Rider assigned successfully!");
        }
        private void ShowSuccessSnackbar(string message)
        {
            SuccessSnackbar.MessageQueue?.Enqueue(message);
        }
    }
}
