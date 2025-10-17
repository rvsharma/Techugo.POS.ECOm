using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            //string formattedDate = "2025-09-30";

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
                        OrderDetailsReponse orderDetails = await _apiService.GetAsync<OrderDetailsReponse>("order/order-detail/" + o.OrderID);

                        if (orderDetails.Data != null)
                        {
                            var data = orderDetails.Data;
                            var address = data.AddressList.HouseNo.ToString() + ", "
                                        + data.AddressList.StreetNo.ToString() + ", "
                                        + data.AddressList.State.ToString() + ", "
                                        + data.AddressList.City.ToString() + ", "
                                        + data.AddressList.Pincode.ToString();
                            OrderDetailVM order = new OrderDetailVM();
                            order.OrderID = data.OrderID;
                            order.OrderNo = data.OrderNo;
                            order.createdAt = data.createdAt;
                            order.ExpectedDeliveryDate = data.ExpectedDeliveryDate;
                            order.TotalAmount = data.TotalAmount;
                            order.PaidAmount = data.PaidAmount;
                            order.Status = data.Status;
                            order.Address = data.AddressList.HouseNo.ToString() + ", "
                                            + data.AddressList.StreetNo.ToString() + ", "
                                            + data.AddressList.State.ToString() + ", "
                                            + data.AddressList.City.ToString() + ", "
                                            + data.AddressList.Pincode.ToString();
                            order.ShortAddress = address.Length > 20 ? address.Substring(0, 20) + "..." : address;
                            order.PaymentMode = data.PaymentMode;
                            order.Subscription = data.Subscription;
                            order.OrderDetails = data.OrderDetails;
                            order.Customer = data.Customer;
                            order.BranchDeliverySlot = o.BranchDeliverySlot.StartTime + " - " + o.BranchDeliverySlot.EndTime;
                            order.ItemImages = o.ItemImages;
                            order.Status = o.Status;
                            order.Items = data.OrderDetails.Count + " items(s)";
                            orderData.Add(order);
                        }
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
                Width = SystemParameters.PrimaryScreenWidth,
                Height = SystemParameters.PrimaryScreenHeight,
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
