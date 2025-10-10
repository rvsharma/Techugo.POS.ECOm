using System;
using System.Collections.Generic;
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
