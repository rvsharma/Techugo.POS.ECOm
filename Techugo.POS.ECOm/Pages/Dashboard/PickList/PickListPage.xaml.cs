using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Pages.Dashboard;
using Techugo.POS.ECOm.Pages.Dashboard.PickList;

namespace Techugo.POS.ECOm.Pages
{
    /// <summary>
    /// Interaction logic for PickListPage.xaml
    /// </summary>
    public partial class PickListPage : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        public event PropertyChangedEventHandler PropertyChanged;
        private Window _editPickListPopUpWindow;

        private readonly ApiService _apiService;
        // public ObservableCollection<PickListOrder> PickListOrders { get; set; } = new();
        private ObservableCollection<PickListOrder> _pickListOrders;
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
        public ObservableCollection<PickListOrder> PickListOrders
        {
            get => _pickListOrders;
            set
            {
                _pickListOrders = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PickListOrders)));
            }
        }
        private string _pickListOrderText;
        public string PickListOrderText
        {
            get => _pickListOrderText;
            set
            {
                _pickListOrderText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PickListOrderText)));
            }
        }
        public PickListPage()
        {
            InitializeComponent();
            DataContext = this;
            PickListOrders = new ObservableCollection<PickListOrder>();
            _apiService = ApiServiceFactory.Create();
            LoadPickListData();
        }

        private async void LoadPickListData()
        {
            //string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            string formattedDate = "2025-10-08";

            AssignRiderOrdersResponse assignRiderOrdersResponse = await _apiService.GetAsync<AssignRiderOrdersResponse>("order/orders-list-by-zone?OrderType=OneTime&page=1&limit=10&status=PickListOrder&Date=" + formattedDate + "");
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
                PickListOrders.Clear();
                foreach (var groupedOrder in GroupedOrders)
                {
                    foreach (var o in groupedOrder.Orders)
                    {
                        OrderDetailsReponse orderDetails = await _apiService.GetAsync<OrderDetailsReponse>("order/order-detail/" + o.OrderID);
                        if (orderDetails.Data != null)
                        {
                            PickListOrders.Add(new PickListOrder
                            {
                                OrderID = orderDetails.Data.OrderID,
                                OrderNo = orderDetails.Data.OrderNo,
                                CustomerName = orderDetails.Data.Customer.CustomerName,
                                TotalItems = orderDetails.Data.OrderDetails.Count,
                                OrderValue = orderDetails.Data.TotalAmount,
                                //ActionsText = "Collapse Details",
                                IsExpanded = false,
                                Items = orderDetails.Data.OrderDetails.Select(od => new PickListItem
                                {
                                    ItemID = od.ItemID,
                                    ItemName = od.Item.ItemName,
                                    Size = od.Size,
                                    Qty = od.Quantity,
                                    EditQty = od.Quantity,
                                    Weight = od.Size,
                                    UOM = od.UOM,
                                    Rate = od.Amount,
                                    Total = od.NetAmount
                                }).ToList()
                            });
                        }
                    }

                }

                PickListOrderText = $"Pick List Orders ({PickListOrders.Count} orders, {PickListOrders.Sum(o => o.Items?.Count ?? 0)} items)";

            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }

        private void EditQty_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            object candidate = button.DataContext ?? button.CommandParameter ?? button.Tag ?? GetDataContextFromAncestors(button);

            EditQtyViewModel? itemDetails = candidate as EditQtyViewModel;

            if (itemDetails == null && candidate is PickListItem pli)
            {
                itemDetails = new EditQtyViewModel
                {
                    TitleText = "Edit Qty - " + pli.ItemName,         // use pli, not itemDetails
                    SKU = "PLI-002",
                    ItemID = pli.ItemID,
                    ItemName = pli.ItemName,
                    OrderedQty = $"{pli.Qty}{pli.UOM}",               // safer formatting
                    MeasuredQty = pli.EditQty,
                    MeasuredWeight = pli.Weight,
                    OUM = pli.UOM,
                    PricePerKg = pli.Rate
                };
            }

            if (itemDetails == null)
            {
                // defensive: no valid data to show
                // log/debug here and return gracefully
                return;
            }
            var popup = new EditPickList(itemDetails);
            popup.CloseClicked += CloseOrderDetailsPopUp;

            // Option 1: Show as overlay in PageContent (replace current content)
            // SetPageContent(popup);

            // Option 2: Show as a dialog/modal (recommended for popups)
            // If you want a true modal, consider using a Window or a custom overlay.
            // Example:
            _editPickListPopUpWindow = new Window
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
            _editPickListPopUpWindow.ShowDialog();
        }
        private void CloseOrderDetailsPopUp(object sender, RoutedEventArgs e)
        {
            if (_editPickListPopUpWindow != null)
            {
                _editPickListPopUpWindow.Close();
                _editPickListPopUpWindow = null;
            }
        }
        private static object GetDataContextFromAncestors(DependencyObject start)
        {
            var current = start;
            while (current != null)
            {
                if (current is FrameworkElement fe && fe.DataContext != null)
                    return fe.DataContext;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }


}
