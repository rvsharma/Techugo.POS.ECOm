using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using Techugo.POS.ECOm.Services;

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
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            // string formattedDate = "2025-10-08";

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
                                    Weight = Convert.ToDecimal(od.Size),
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
                    TitleText = "Edit Qty - " + pli.ItemName,
                    SKU = "PLI-002",
                    ItemID = pli.ItemID,
                    ItemName = pli.ItemName,
                    OrderedQty = $"{pli.Qty}{pli.UOM}",
                    MeasuredQty = pli.EditQty,
                    MeasuredWeight = pli.Weight.ToString(CultureInfo.CurrentCulture),
                    OUM = pli.UOM,
                    PricePerKg = pli.Rate,
                    OriginalAmount = pli.Rate
                };
            }

            if (itemDetails == null)
            {
                return;
            }

            var popup = new EditPickList(itemDetails);
            popup.CloseClicked += CloseOrderDetailsPopUp;

            // subscribe to SaveClicked to receive the updated model when Save is pressed
            popup.SaveClicked += Popup_SaveClicked;

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

        private async void Popup_SaveClicked(object? sender, RoutedEventArgs e)
        {
            // sender should be the EditPickList control
            if (sender is not EditPickList popup) return;
            var vm = popup.ItemDetails;
            if (vm == null) return;

            // parse numeric weight from vm.MeasuredWeight (it may be "1.23 kg" or "1.23")
            string weightText = vm.MeasuredWeight ?? string.Empty;
            if (weightText.EndsWith("kg", StringComparison.OrdinalIgnoreCase))
                weightText = weightText.Substring(0, weightText.Length - 2).Trim();

            if (!decimal.TryParse(weightText, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal newWeight))
            {
                // If parsing fails, you can notify user — for now just return
                MessageBox.Show("Invalid weight entered", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Find the PickListOrder and PickListItem that match the edited ItemID
            PickListItem? targetItem = null;
            PickListOrder? parentOrder = null;

            foreach (var order in PickListOrders)
            {
                var match = order.Items?.FirstOrDefault(i => i.ItemID == vm.ItemID);
                if (match != null)
                {
                    targetItem = match;
                    parentOrder = order;
                    break;
                }
            }

            if (targetItem == null || parentOrder == null)
            {
                // Could not find item in current in-memory list; still attempt API or notify
                MessageBox.Show("Could not locate the item to update in the current list.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Close popup anyway
                CloseOrderDetailsPopUp(popup, new RoutedEventArgs());
                return;
            }

            // Update in-memory model
            targetItem.Weight = newWeight;

            // Recalculate any totals if needed (e.g., Total) — adjust based on your model
            // Example: targetItem.Total = targetItem.Rate * targetItem.Qty; // if applicable

            // Refresh UI by notifying property change for the collection
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PickListOrders)));

            // Prepare payload for API — replace with your actual expected schema
            var payload = new
            {
                OrderID = parentOrder.OrderID,
                ItemID = targetItem.ItemID,
                MeasuredWeight = newWeight,
                MeasuredQty = vm.MeasuredQty,           // may be int or decimal per your API
                MeasuredAmount = vm.MeasuredAmount
            };

            try
            {
                var result = await _apiService.PutAsync<BaseResponse>($"order/update-item/{parentOrder.OrderID}/{targetItem.ItemID}", payload);
                if (result != null && result.Success)
                {
                    SnackbarService.Enqueue("Item updated");
                }
                else
                {
                    SnackbarService.Enqueue($"Failed to update item: {(result?.Message ?? "Unknown error")}");
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Enqueue($"Error updating item: {ex.Message}");
            }

            // Close popup window
            CloseOrderDetailsPopUp(popup, new RoutedEventArgs());
        }

        private async void Ready_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var selectable = btn?.DataContext as SelectableOrderDetail;
            var order = selectable?.Item;
            if (order == null) return;

            var orderID = Convert.ToInt32(order.OrderID);
            var data = new { OrderIDs = new[] { orderID }, BranchStatus = "Packed" };
            BaseResponse result = await _apiService.PutAsync<BaseResponse>("order/update-order", data);
            if (result != null)
            {
                if (result.Success == true)
                {
                    SnackbarService.Enqueue($"Orders Accepted");

                }
            }
        }
    }


}
