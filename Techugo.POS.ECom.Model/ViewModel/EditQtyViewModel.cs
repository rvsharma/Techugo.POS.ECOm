using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string MeasuredWeight { get; set; }
        public decimal SPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Discount { get; set; }
        public int MeasuredQty { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal MeasuredAmount { get; set; }

        public string PricePerKgDisplay => string.Format(CultureInfo.CurrentCulture, "₹{0:N0}", SPrice);
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
