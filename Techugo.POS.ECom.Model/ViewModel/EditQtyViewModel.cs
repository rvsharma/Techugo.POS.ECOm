using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Techugo.POS.ECom.Model.ViewModel
{
    public class EditQtyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string OrderID { get; set; }
        public string OrderDetailID { get; set; }
        public string ItemName { get; set; }
        public int ItemID { get; set; }
        public int OrderedQty { get; set; }
        public int EditedQty { get; set; }
        public string OrderedQtyDisPlay { get; set; }
        public string SKU { get; set; }
        public string UOM { get; set; }
        public decimal SPrice { get; set; }
        public decimal OriginalAmount { get; set; }
        private decimal _weight;
        public decimal Weight
        {
            get => _weight;
            set
            {
                if (_weight == value) return;
                _weight = value;
                Notify(nameof(Weight));
            }
        }
        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount == value) return;
                _amount = value;
                Notify(nameof(Amount));
                UpdateDifference(); // recompute when Amount changes
            }
        }

        private decimal _measuredAmount;
        public decimal MeasuredAmount
        {
            get => _measuredAmount;
            set
            {
                if (_measuredAmount == value) return;
                _measuredAmount = value;
                Notify(nameof(MeasuredAmount));
                UpdateDifference(); // recompute when MeasuredAmount changes
            }
        }

        private string _differenceAmount;
        public string DifferenceAmount
        {
            get => _differenceAmount;
            set
            {
                if (_differenceAmount == value) return;
                _differenceAmount = value;
                Notify(nameof(DifferenceAmount));
            }
        }

        private bool _isDifferenceNegative;
        public bool IsDifferenceNegative
        {
            get => _isDifferenceNegative;
            private set
            {
                if (_isDifferenceNegative == value) return;
                _isDifferenceNegative = value;
                Notify(nameof(IsDifferenceNegative));
            }
        }
        private void UpdateDifference()
        {
            // measured minus original amount
            DifferenceAmount = (MeasuredAmount - OriginalAmount).ToString("0.##", CultureInfo.CurrentCulture); ;
            IsDifferenceNegative = (MeasuredAmount - OriginalAmount) < 0;
        }

        // Validation message (empty/null => no message shown)
        private string _validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => _validationMessage;
            set { _validationMessage = value ?? string.Empty; Notify(nameof(ValidationMessage)); }
        }

        private bool _canSave = true;
        public bool CanSave
        {
            get => _canSave;
            set { if (_canSave == value) return; _canSave = value; Notify(nameof(CanSave)); }
        }

        protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));




        private void Revalidate()
        {
            // No validation message when input is empty
            //if (string.IsNullOrWhiteSpace(MeasuredQtyText))
            //{
            //    ValidationMessage = "Quantity cannot be empty.";
            //    CanSave = false;
            //    return;
            //}

            //// Show validation and disable save when measured is greater than ordered qty
            //if (_measuredQty > OrderedQtyDecimal)
            //{
            //    ValidationMessage = "Quantity cannot be greater than ordered quantity.";
            //    CanSave = false;
            //}
            //else
            //{
            //    ValidationMessage = string.Empty;
            //    CanSave = true;
            //}
        }
    }
}