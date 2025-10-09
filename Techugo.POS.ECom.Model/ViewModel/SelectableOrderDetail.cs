using System;
using System.ComponentModel;

namespace Techugo.POS.ECom.Model.ViewModel
{
    public class SelectableOrderDetail : INotifyPropertyChanged
    {
        public OrderDetailVM Item { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public SelectableOrderDetail(OrderDetailVM item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}