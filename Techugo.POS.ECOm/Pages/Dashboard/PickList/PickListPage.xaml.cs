using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Specialized;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Pages.Dashboard.PickList;
using Techugo.POS.ECOm.Pages.Dashboard.PendingRequest;
using Techugo.POS.ECOm.Services;
using Techugo.POS.ECOm.Helper;
using Techugo.POS.ECOm.Convertors;

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
        private Window _rejectOrderPopUpWindow;

        private readonly ApiService _api_service;
        // public ObservableCollection<PickListOrder> PickListOrders { get; set; } = new();
        private ObservableCollection<PickListOrder> _pickListOrders;
        private ObservableCollection<SocietyOrderGroup1> _groupedOrders;

        public ObservableCollection<SocietyOrderGroup1> GroupedOrders
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
            NoItemsText.Visibility = Visibility.Collapsed;
            DataContext = this;
            PickListOrders = new ObservableCollection<PickListOrder>();
            _api_service = ApiServiceFactory.Create();
            LoadPickListData();
        }

        private async void LoadPickListData()
        {
            string formattedDate = GlobalData.SelectedDashboardDate.ToString("yyyy-MM-dd");
             //string formattedDate = "2025-02-13";
            try
            {
                PickListResponse assignRiderOrdersResponse = await _api_service.GetAsync<PickListResponse>("order/orders-list-by-zone?OrderType=OneTime&page=1&limit=1000&status=PickListOrder&Date=" + formattedDate + "");
                if (assignRiderOrdersResponse != null)
                {

                    var grouped = assignRiderOrdersResponse.Data
                 .GroupBy(g => g.Society)
                 .Select(grp => new SocietyOrderGroup1
                 {
                     Society = grp.Key,
                     Type = grp.First().Type,
                     Zone = grp.First().Zone,
                     Orders = grp.SelectMany(x => x.Orders).ToList()
                 })
                 .ToList();

                    GroupedOrders = new ObservableCollection<SocietyOrderGroup1>(grouped);
                    PickListOrders.Clear();
                    foreach (var groupedOrder in GroupedOrders)
                    {
                        foreach (var o in groupedOrder.Orders)
                        {
                            OrderDetailsReponse orderDetails = await _api_service.GetAsync<OrderDetailsReponse>("order/order-detail/" + o.OrderID);
                            if (orderDetails.Data != null)
                            {
                                // build items collection
                                var items = new ObservableCollection<PickListItem>(
                                    orderDetails.Data.OrderDetails.Select(od => new PickListItem
                                    {
                                        OrderID = orderDetails.Data.OrderID,
                                        OrderDetailID = od.ID,
                                        ItemID = od.ItemID,
                                        ItemName = od.Item.ItemName.Length > 20 ? od.Item.ItemName.Substring(0, 20) + "..." : od.Item.ItemName,
                                        ItemFullName = od.Item.ItemName,
                                        Size = od.Size,
                                        Qty = od.Quantity,
                                        EditQty = od.Quantity,
                                        Weight = od.Size,
                                        UOM = od.UOM,
                                        SPrice = od.SPrice,
                                        Amount = od.Amount,
                                        OriginalAmount = od.Quantity * od.SPrice,
                                        NetAmount = od.NetAmount,
                                        Discount = od.Discount,
                                        OriginalDiscount = od.Discount,
                                        ImageUrl = od.Item.ItemImages[0].ImagePath,
                                        IsLooseItem = od.IsLooseItem
                                    }).ToList()
                                );

                                // create order
                                var order = new PickListOrder
                                {
                                    OrderID = orderDetails.Data.OrderID,
                                    OrderNo = orderDetails.Data.OrderNo,
                                    CustomerName = o.OrderAddress?.Name,
                                    TotalItems = orderDetails.Data.OrderDetails.Count,
                                    OrderValue = items?.Sum(i => i.NetAmount) ?? orderDetails.Data.TotalAmount,
                                    IsExpanded = false,
                                    Items = items
                                };

                                // attach handlers so OrderValue updates when item Amount changes or items collection changes
                                AttachItemHandlers(order);

                                PickListOrders.Add(order);
                            }
                        }

                    }
                    if(PickListOrders.Count == 0)
                    {
                        NoItemsText.Visibility = Visibility.Visible;
                        PickListHeader.Visibility = Visibility.Collapsed;
                    }

                    PickListOrderText = $"Pick List Orders ({PickListOrders.Count} orders, {PickListOrders.Sum(o => o.Items?.Count ?? 0)} items)";

                }
            }
            catch(Exception ex) {
            
            }
            
        }

        private void AttachItemHandlers(PickListOrder order)
        {
            if (order == null || order.Items == null) return;

            // attach to existing items
            foreach (var item in order.Items.ToList())
            {
                AttachPropertyChanged(item);
            }

            // monitor collection changes (add/remove/replace)
            // use a named handler so we could remove it later if needed
            NotifyCollectionChangedEventHandler collectionHandler = (s, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (var old in e.OldItems.Cast<PickListItem>())
                        DetachPropertyChanged(old);
                }
                if (e.NewItems != null)
                {
                    foreach (var n in e.NewItems.Cast<PickListItem>())
                        AttachPropertyChanged(n);
                }

                // Recalculate whenever items added/removed/replaced
                order.OrderValue = order.Items?.Sum(i => i.Amount) ?? 0m;
            };

            // prevent multiple subscriptions: remove existing first if present
            order.Items.CollectionChanged -= collectionHandler;
            order.Items.CollectionChanged += collectionHandler;
        }

        private void AttachPropertyChanged(PickListItem item)
        {
            if (item is INotifyPropertyChanged inpc)
            {
                // Use PropertyChangedEventManager to avoid strong event handler references
                PropertyChangedEventManager.AddHandler(inpc, OnItemPropertyChanged, string.Empty);
            }
        }

        private void DetachPropertyChanged(PickListItem item)
        {
            if (item is INotifyPropertyChanged inpc)
            {
                PropertyChangedEventManager.RemoveHandler(inpc, OnItemPropertyChanged, string.Empty);
            }
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // respond when Amount changes (or when PropertyName is empty/null, which means many properties changed)
            if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName != nameof(PickListItem.Amount))
                return;

            if (sender is not PickListItem item) return;

            // find parent order that contains this item
            var parent = PickListOrders?.FirstOrDefault(o => o.Items != null && o.Items.Contains(item));
            if (parent != null)
            {
                parent.OrderValue = parent.Items?.Sum(i => i.Amount) ?? parent.OrderValue;
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
                    OrderID = pli.OrderID,
                    OrderDetailID = pli.OrderDetailID,
                    SKU = "PLI-002",
                    ItemID = pli.ItemID,
                    ItemName = pli.ItemName,
                    OrderedQty = pli.Qty,
                    EditedQty = pli.EditQty,
                    OrderedQtyDisPlay = pli.Qty.ToString(),
                    Weight = (pli.IsLooseItem && pli.Weight != null && decimal.TryParse(pli.Weight, out var w)) ? w : 0m,
                    UOM = pli.UOM,
                    SPrice = pli.SPrice,
                    Amount = pli.Amount,
                    OriginalAmount = pli.OriginalAmount, // Always show the original order total
                    MeasuredAmount = pli.IsLooseItem ? 0m : pli.Amount, // For loose items, start at 0 to prompt weighing
                    IsLooseItem = pli.IsLooseItem,
                    IsEdited = pli.IsEdited
                };

                // Dynamic Unit Factor Calculation
                // Handles cases where units mismatch (e.g. Price is per Kg but Weight/Qty is in grams)
                // Always base the calculation on the ORIGINAL API amount and original Size, not the currently edited amount/weight
                decimal initialWeight = 1m;
                if (pli.IsLooseItem && !string.IsNullOrEmpty(pli.Size))
                {
                    // Extract numeric part from original Size (in case it contains letters like "500G")
                    var cleanSize = System.Text.RegularExpressions.Regex.Replace(pli.Size, @"[^0-9.]", "");
                    if (decimal.TryParse(cleanSize, out var parsedS))
                        initialWeight = parsedS;
                }
                
                decimal initialCalc = pli.Qty * pli.SPrice * initialWeight;
                if (initialCalc != 0 && Math.Abs(pli.OriginalAmount - initialCalc) > 0.01m)
                {
                    if (Math.Abs(pli.OriginalAmount - (initialCalc / 1000m)) < 0.01m)
                        itemDetails.UnitFactor = 0.001m;
                    else if (Math.Abs(pli.OriginalAmount - (initialCalc * 1000m)) < 0.01m)
                        itemDetails.UnitFactor = 1000m;
                    else
                        itemDetails.UnitFactor = pli.OriginalAmount / initialCalc;
                }
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

        private void Popup_SaveClicked(object? sender, RoutedEventArgs e)
        {
            // sender should be the EditPickList control
            if (sender is not EditPickList popup) return;
            var vm = popup.ItemDetails;
            if (vm == null) return;

            // parse numeric weight from vm.MeasuredWeight (it may be "1.23 kg" or "1.23")
           
            // Find the PickListOrder and PickListItem that match the edited ItemID
            PickListItem? targetItem = null;
            PickListOrder? parentOrder = null;

            foreach (var order in PickListOrders)
            {
                var match = order.Items?.FirstOrDefault(i => i.OrderDetailID == vm.OrderDetailID);
                if (match != null)
                {
                    targetItem = match;
                    parentOrder = order;
                    break;
                }
            }

            if (targetItem == null || parentOrder == null)
            {
                MessageBox.Show("Could not locate the item to update in the current list.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                CloseOrderDetailsPopUp(popup, new RoutedEventArgs());
                return;
            }

            // Create a new item instance with updated values and replace it in the observable collection
            // Discount remains exactly the same as originally ordered, no proportional recalculation
            decimal newAmount = vm.MeasuredAmount;
            decimal newDiscount = targetItem.OriginalDiscount;
            decimal newNetAmount = newAmount - newDiscount;

            var updatedItem = new PickListItem
            {
                OrderDetailID = targetItem.OrderDetailID,
                ItemID = targetItem.ItemID,
                ItemName = targetItem.ItemName,
                Size = targetItem.Size,
                Qty = targetItem.Qty,
                EditQty = vm.EditedQty,
                Weight = Convert.ToString(vm.Weight == 0m ? targetItem.Size : vm.Weight),
                SPrice = targetItem.SPrice,
                Amount = newAmount,
                OriginalAmount = targetItem.OriginalAmount,
                Discount = newDiscount,
                OriginalDiscount = targetItem.OriginalDiscount,
                NetAmount = newNetAmount,
                Total = targetItem.Total,
                ImageUrl = targetItem.ImageUrl,
                IsEdited = true // Mark as edited so quantity can't be changed again
            };

            // Replace item in the parent's Items collection to raise CollectionChanged and update UI
            var itemIndex = parentOrder.Items?.IndexOf(targetItem) ?? -1;
            if (itemIndex >= 0)
            {
                parentOrder.Items[itemIndex] = updatedItem;
            }

            // Recalculate parent order totals (collection change handler will also recalc; this is defensive)
            // Recalculate parent order totals using NetAmount to respect discounts
            parentOrder.OrderValue = parentOrder.Items?.Sum(i => i.NetAmount) ?? parentOrder.OrderValue;

            var orderIndex = PickListOrders.IndexOf(parentOrder);
            if (orderIndex >= 0)
            {
                // replace the order to raise CollectionChanged on PickListOrders (UI will refresh header fields)
                PickListOrders[orderIndex] = parentOrder;
            }

            // Close popup window
            CloseOrderDetailsPopUp(popup, new RoutedEventArgs());

            // Check if no items left (all items have 0 quantity)
            if (parentOrder.Items != null && parentOrder.Items.All(i => i.EditQty <= 0))
            {
                ShowRejectionPopup(parentOrder);
            }
        }

        private void ShowRejectionPopup(PickListOrder order)
        {
            var orderDetailVM = new OrderDetailVM
            {
                OrderID = order.OrderID,
                OrderNo = order.OrderNo
            };
            var popUpData = new SelectableOrderDetail(orderDetailVM);

            var popup = new RejectOrderPopUp(popUpData);
            popup.CloseClicked += (s, e) => {
                _rejectOrderPopUpWindow?.Close();
                _rejectOrderPopUpWindow = null;
            };
            popup.PendingRequestClick += (s, e) => {
                _rejectOrderPopUpWindow?.Close();
                _rejectOrderPopUpWindow = null;
                LoadPickListData(); // Refresh list
                SnackbarService.Enqueue($"Order {order.OrderNo} Cancelled Successfully");
            };

            _rejectOrderPopUpWindow = new Window
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
            _rejectOrderPopUpWindow.ShowDialog();
        }

        private async void Ready_Button_Click(object sender, RoutedEventArgs e)
        {

            var btn = sender as Button;
            var selectable = btn?.DataContext;
            if (selectable == null) return;

            // Fix: Cast selectable to PickListOrder before accessing OrderID
            if (selectable is PickListOrder order)
            {
                // Validation: Ensure all loose items have been explicitly verified/edited
                var uneditedLoose = order.Items?.FirstOrDefault(i => i.IsLooseItem && !i.IsEdited);
                if (uneditedLoose != null)
                {
                    SnackbarService.Enqueue($"Please edit the loose items.");
                    return;
                }

                // Validation: Check if all items have 0 quantity
                if (order.Items != null && order.Items.All(i => i.EditQty <= 0))
                {
                    ShowRejectionPopup(order);
                    return;
                }

                var orderID = order.OrderID;
                var selectedOrder = PickListOrders.First(x => x.OrderID == orderID);
               

                var items = selectedOrder.Items?
                    .Select(i => new
                    {
                        // OrderDetailID as int when possible (matches example)
                        OrderDetailID = int.TryParse(i.OrderDetailID, out var odid) ? odid : (object)i.OrderDetailID,
                        // Quantity comes from EditQty (edited quantity)
                        Quantity = i.EditQty,
                        // Size or null
                        Size = string.IsNullOrWhiteSpace(i.Weight) ? null : i.Weight,
                        // Original amount (use item.Amount or another property if you store original separately)
                        OrginalAmount = i.Qty * i.SPrice,
                        // MeasuredAmount — if you track measured separately, use that; fallback to Amount
                        MeasuredAmount = i.Amount
                    })
                    .ToList();

                var itemsData = new
                {
                    OrderID =Convert.ToInt32(orderID),
                    Items = items
                };

                BaseResponse itemsSaveResult = await _api_service.PutAsync<BaseResponse>("order/edit-order-item", itemsData);
                if(itemsSaveResult != null && itemsSaveResult.Success == true)
                {
                    var data = new { OrderIDs = new[] { orderID }, BranchStatus = "Packed" };
                    BaseResponse result = await _api_service.PutAsync<BaseResponse>("order/update-order", data);
                    if (result != null)
                    {
                        if (result.Success == true)
                        {
                            SnackbarService.Enqueue($"Order {order.OrderNo} Packed");
                            var main = Application.Current.MainWindow;
                            LayoutPage? layout = null;

                            // Case A: MainWindow.Content is LayoutPage
                            if (main?.Content is LayoutPage lp)
                                layout = lp;
                            else
                            {
                                // Case B: search visual tree for LayoutPage
                                layout = FindChild<LayoutPage>(main);
                            }

                            if (layout != null)
                            {
                                // Use the public method to navigate
                                layout.ShowPickListPage();
                            }
                            else
                            {
                                // Fallback: set PageContent via reflection if LayoutPage is hosted differently
                                var setPageMethod = main?.GetType().GetMethod("SetPageContent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                                if (setPageMethod != null)
                                {
                                    var newPickList = new PickListPage();
                                    setPageMethod.Invoke(main, new object[] { newPickList });
                                }
                            }
                        }
                    }
                }
                else
                {
                    SnackbarService.Enqueue(itemsSaveResult.Message);
                }
            }
        }
        private static T? FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typed) return typed;
                var found = FindChild<T>(child);
                if (found != null) return found;
            }
            return null;
        }
    }
}
