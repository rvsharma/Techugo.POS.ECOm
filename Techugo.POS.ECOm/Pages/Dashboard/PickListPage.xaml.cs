using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages
{
    /// <summary>
    /// Interaction logic for PickListPage.xaml
    /// </summary>
    public partial class PickListPage : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        public event PropertyChangedEventHandler PropertyChanged;

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
            string formattedDate = "2025-10-04";

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
    }

    public class PickListOrder : INotifyPropertyChanged
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public string CustomerName { get; set; }
        public int TotalItems { get; set; }
        public decimal OrderValue { get; set; }
        public string ActionsText
        {
            get => IsExpanded ? "Collapse Details" : "Expand Details";
        }
        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ActionsText));
                }
            }
        }
        public List<PickListItem> Items { get; set; }

        public ICommand ToggleExpandCommand { get; }

        public PickListOrder()
        {
            ToggleExpandCommand = new RelayCommand(() => IsExpanded = !IsExpanded);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class PickListItem
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public int EditQty { get; set; }
        public string Weight { get; set; }
        public string UOM { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
    }
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
    }
}
