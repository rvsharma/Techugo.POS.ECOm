using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Techugo.POS.ECom.Model.ViewModel
{
    public class PickListOrder : INotifyPropertyChanged
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public string CustomerName { get; set; }
        public int TotalItems { get; set; }

        private decimal _orderValue;
        public decimal OrderValue
        {
            get => _orderValue;
            set
            {
                if (_orderValue == value) return;
                _orderValue = value;
                OnPropertyChanged();
            }
        }

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

        private ObservableCollection<PickListItem> _items;
        public ObservableCollection<PickListItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public ICommand ToggleExpandCommand { get; }

        public PickListOrder()
        {
            _items = new ObservableCollection<PickListItem>();
            ToggleExpandCommand = new RelayCommand(() => IsExpanded = !IsExpanded);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class PickListItem : INotifyPropertyChanged
    {
        private string _orderID;
        public string OrderID
        {
            get => _orderID;
            set { if (_orderID == value) return; _orderID = value; OnPropertyChanged(); }
        }
        private string _orderDetailID;
        public string OrderDetailID
        {
            get => _orderDetailID;
            set { if (_orderDetailID == value) return; _orderDetailID = value; OnPropertyChanged(); }
        }

        private int _itemID;
        public int ItemID
        {
            get => _itemID;
            set { if (_itemID == value) return; _itemID = value; OnPropertyChanged(); }
        }

        private string _itemName;
        public string ItemName
        {
            get => _itemName;
            set { if (_itemName == value) return; _itemName = value; OnPropertyChanged(); }
        }

        private string _size;
        public string Size
        {
            get => _size;
            set { if (_size == value) return; _size = value; OnPropertyChanged(); }
        }

        private int _qty;
        public int Qty
        {
            get => _qty;
            set { if (_qty == value) return; _qty = value; OnPropertyChanged(); }
        }

        private int _editQty;
        public int EditQty
        {
            get => _editQty;
            set { if (_editQty == value) return; _editQty = value; OnPropertyChanged(); }
        }

        private string _weight;
        public string Weight
        {
            get => _weight;
            set { if (_weight == value) return; _weight = value; OnPropertyChanged(); }
        }

        private string _uom;
        public string UOM
        {
            get => _uom;
            set { if (_uom == value) return; _uom = value; OnPropertyChanged(); }
        }

        private decimal _sPrice;
        public decimal SPrice
        {
            get => _sPrice;
            set { if (_sPrice == value) return; _sPrice = value; OnPropertyChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount == value) return;
                _amount = value;
                OnPropertyChanged();
            }
        }

        private decimal _netAmount;
        public decimal NetAmount
        {
            get => _netAmount;
            set { if (_netAmount == value) return; _netAmount = value; OnPropertyChanged(); }
        }

        private decimal _discount;
        public decimal Discount
        {
            get => _discount;
            set { if (_discount == value) return; _discount = value; OnPropertyChanged(); }
        }

        private decimal _rate;
        public decimal Rate
        {
            get => _rate;
            set { if (_rate == value) return; _rate = value; OnPropertyChanged(); }
        }

        private decimal _total;
        public decimal Total
        {
            get => _total;
            set { if (_total == value) return; _total = value; OnPropertyChanged(); }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get => _imageUrl;
            set { if (_imageUrl == value) return; _imageUrl = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
