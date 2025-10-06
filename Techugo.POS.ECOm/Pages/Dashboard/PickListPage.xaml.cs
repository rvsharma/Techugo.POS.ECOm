using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Techugo.POS.ECOm.Pages
{
    /// <summary>
    /// Interaction logic for PickListPage.xaml
    /// </summary>
    public partial class PickListPage : UserControl
    {
        public event RoutedEventHandler BackRequested;
        public ObservableCollection<PickListOrder> PickListOrders { get; set; } = new();

        public PickListPage()
        {
            InitializeComponent();
            DataContext = this;

            PickListOrders.Add(new PickListOrder
            {
                OrderNo = "ORD-001",
                CustomerName = "Rahul Sharma",
                TotalItems = 2,
                OrderValue = 790,
                //ActionsText = "Collapse Details",
                IsExpanded = true,
                Items = new List<PickListItem>
                {
                    new PickListItem
                    {
                        ItemName = "Cooking Oil - Sunflower",
                        Size = "1L",
                        Qty = 2,
                        EditQty = 2,
                        Weight = 1.80,
                        Rate = 170,
                        Total = 340
                    },
                    new PickListItem
                    {
                        ItemName = "Basmati Rice Premium",
                        Size = "5kg",
                        Qty = 1,
                        EditQty = 1,
                        Weight = 4.80,
                        Rate = 450,
                        Total = 450
                    }
                }
            });

            PickListOrders.Add(new PickListOrder
            {
                OrderNo = "ORD-002",
                CustomerName = "Priya Patel",
                TotalItems = 2,
                OrderValue = 275,
                //ActionsText = "Expand Details",
                IsExpanded = false,
                Items = new List<PickListItem>
                {
                    new PickListItem
                    {
                        ItemName = "Sugar",
                        Size = "1kg",
                        Qty = 1,
                        EditQty = 1,
                        Weight = 1.00,
                        Rate = 50,
                        Total = 50
                    },
                    new PickListItem
                    {
                        ItemName = "Salt",
                        Size = "1kg",
                        Qty = 1,
                        EditQty = 1,
                        Weight = 1.00,
                        Rate = 25,
                        Total = 25
                    }
                }
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }

    public class PickListOrder : INotifyPropertyChanged
    {
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
        public string ItemName { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public int EditQty { get; set; }
        public double Weight { get; set; }
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
