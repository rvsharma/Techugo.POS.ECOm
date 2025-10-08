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

        public string TitleText { get; set; }
        public string OUM { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public string OrderedQty { get; set; }
        public string MeasuredWeight { get; set; }
        public int MeasuredQty { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal MeasuredAmount { get; set; }

        public string PricePerKgDisplay => string.Format(CultureInfo.CurrentCulture, "₹{0:N0}", PricePerKg);
        public string OriginalAmountDisplay => string.Format(CultureInfo.CurrentCulture, "₹{0:N2}", OriginalAmount);
        public string MeasuredAmountDisplay => string.Format(CultureInfo.CurrentCulture, "₹{0:N2}", MeasuredAmount);
        public string DifferenceDisplay => string.Format(CultureInfo.CurrentCulture, "{0}{1:N2}", MeasuredAmount - OriginalAmount >= 0 ? "+" : "-", Math.Abs(MeasuredAmount - OriginalAmount));

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
