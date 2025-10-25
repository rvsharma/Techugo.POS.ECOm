using System;
using System.ComponentModel;
using System.Globalization;

namespace Techugo.POS.ECom.Model.ViewModel
{
    public class EditQtyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string OrderDetailID { get; set; }
        public string TitleText { get; set; }
        public string OUM { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public string OrderedQty { get; set; }

        // MeasuredWeight remains a separate field (string with unit)
        public string MeasuredWeight { get; set; }

        // SPrice is per-unit price used when MeasuredQty represents quantity
        private decimal _sPrice;
        public decimal SPrice
        {
            get => _sPrice;
            set
            {
                if (_sPrice == value) return;
                _sPrice = value;
                // recalc measured amount using current MeasuredQty
                MeasuredAmount = Math.Round(_sPrice * MeasuredQty, 2);
                Notify(nameof(SPrice));
                UpdateDisplays();
            }
        }

        // Original amount (from server)
        public decimal Amount { get; set; }

        public decimal NetAmount { get; set; }
        public decimal Discount { get; set; }

        // MeasuredQty is decimal (quantity), NOT weight
        private decimal _measuredQty;
        public decimal MeasuredQty
        {
            get => _measuredQty;
            set
            {
                if (_measuredQty == value) return;
                _measuredQty = value;
                // Recalculate measured amount as SPrice * quantity
                MeasuredAmount = Math.Round(SPrice * _measuredQty, 2);
                Notify(nameof(MeasuredQty));
                UpdateDisplays();
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
                UpdateDisplays();
            }
        }

        public string PricePerKgDisplay => string.Format(CultureInfo.CurrentCulture, "₹{0:N2}", SPrice);
        public string OriginalAmountDisplay => string.Format(CultureInfo.CurrentCulture, "₹{0:N2}", Amount);
        public string MeasuredAmountDisplay => string.Format(CultureInfo.CurrentCulture, "₹{0:N2}", MeasuredAmount);
        public string DifferenceDisplay => string.Format(CultureInfo.CurrentCulture, "{0}{1:N2}", MeasuredAmount - Amount >= 0 ? "+" : "-", Math.Abs(MeasuredAmount - Amount));

        public void UpdateDisplays()
        {
            Notify(nameof(PricePerKgDisplay));
            Notify(nameof(OriginalAmountDisplay));
            Notify(nameof(MeasuredAmountDisplay));
            Notify(nameof(DifferenceDisplay));
        }

        protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
