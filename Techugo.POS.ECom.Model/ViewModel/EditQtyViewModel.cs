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

        // existing fields
        public string OrderDetailID { get; set; } = string.Empty;
        public string TitleText { get; set; } = string.Empty;
        public string OUM { get; set; } = string.Empty;
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        //public string OrderedQty { get; set; } = "0";
        public int OriginalQty { get; set; } = 0;
        public string MeasuredWeight { get; set; } = string.Empty;

        private decimal _sPrice;
        public decimal SPrice { get => _sPrice; set { _sPrice = value; Notify(nameof(SPrice)); } }

        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Discount { get; set; }

        // numeric measured qty (used by business logic)
        private decimal _measuredQty;
        public decimal MeasuredQty
        {
            get => _measuredQty;
            set
            {
                if (_measuredQty == value) return;
                _measuredQty = value;
                // update the text representation without retriggering parse logic
                _suppressTextUpdate = true;
                MeasuredQtyText = _measuredQty == 0m ? string.Empty : _measuredQty.ToString(CultureInfo.CurrentCulture);
                _suppressTextUpdate = false;
                Notify(nameof(MeasuredQty));
                RecalculateMeasuredAmount();
                Revalidate();
            }
        }

        private decimal _measuredAmount;
        public decimal MeasuredAmount
        {
            get => _measuredAmount;
            set { _measuredAmount = value; Notify(nameof(MeasuredAmount)); }
        }

        public string PricePerKgDisplay { get; set; } = string.Empty;
        public string OriginalAmountDisplay { get; set; } = string.Empty;
        public string MeasuredAmountDisplay { get; set; } = string.Empty;
        public string DifferenceDisplay { get; set; } = string.Empty;

        // New: text-backed input to avoid binding exceptions when empty
        private string _measuredQtyText = string.Empty;
        private bool _suppressTextUpdate = false;
        public string MeasuredQtyText
        {
            get => _measuredQtyText;
            set
            {
                if (_measuredQtyText == value) return;
                _measuredQtyText = value ?? string.Empty;
                Notify(nameof(MeasuredQtyText));

                if (_suppressTextUpdate)
                    return;

                // Empty text: don't show validation; treat as no numeric input
                if (string.IsNullOrWhiteSpace(_measuredQtyText))
                {
                    _measuredQty = 0m;
                    RecalculateMeasuredAmount();
                    Revalidate();
                    Notify(nameof(MeasuredQty));
                    return;
                }

                // Try parse the numeric input
                if (decimal.TryParse(_measuredQtyText, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
                {
                    _measuredQty = parsed;
                    RecalculateMeasuredAmount();
                    Notify(nameof(MeasuredQty));
                }
                else
                {
                    // Non-numeric input: keep previous numeric value (optional: you can surface parse errors)
                }

                Revalidate();
            }
        }
        private string _orderedQty = "0";
        public string OrderedQty
        {
            get => _orderedQty;
            set
            {
                if (_orderedQty == value) return;
                _orderedQty = value ?? "0";
                Notify(nameof(OrderedQty));

                // Robust parsing: try direct parse, otherwise extract numeric characters (respecting current culture decimal separator)
                decimal parsedQty = 0m;
                var nf = CultureInfo.CurrentCulture.NumberFormat;
                if (!decimal.TryParse(_orderedQty, NumberStyles.Number, CultureInfo.CurrentCulture, out parsedQty))
                {
                    // Build cleaned string that keeps digits, sign, and current culture decimal separator characters
                    var allowed = new[] { nf.NumberDecimalSeparator, nf.NegativeSign, nf.PositiveSign };
                    var sb = new StringBuilder();
                    foreach (var ch in _orderedQty)
                    {
                        if (char.IsDigit(ch) || allowed.Contains(ch.ToString()))
                            sb.Append(ch);
                        else if (char.IsWhiteSpace(ch))
                            break; // stop at whitespace after number (e.g. "5 kg")
                    }

                    var cleaned = sb.ToString();
                    // Trim trailing decimal separator if any
                    if (!string.IsNullOrWhiteSpace(cleaned))
                    {
                        if (cleaned.EndsWith(nf.NumberDecimalSeparator))
                            cleaned = cleaned.TrimEnd(nf.NumberDecimalSeparator.ToCharArray());

                        if (!decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out parsedQty))
                        {
                            parsedQty = 0m;
                        }
                    }
                    else
                    {
                        parsedQty = 0m;
                    }
                }

                // Keep OriginalQty as integer representation of parsedQty (flooring to int)
                try
                {
                    OriginalQty = (int)Math.Floor(parsedQty);
                }
                catch
                {
                    OriginalQty = 0;
                }

                Revalidate();
            }
        }

        // Validation message (empty/null => no message shown)
        private string _validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => _validationMessage;
            private set { _validationMessage = value ?? string.Empty; Notify(nameof(ValidationMessage)); }
        }

        private bool _canSave = true;
        public bool CanSave
        {
            get => _canSave;
            private set { if (_canSave == value) return; _canSave = value; Notify(nameof(CanSave)); }
        }

        protected void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void UpdateDisplays()
        {
            // update any display strings the UI uses
            PricePerKgDisplay = SPrice.ToString("0.##", CultureInfo.CurrentCulture);
            OriginalAmountDisplay = Amount.ToString("0.##", CultureInfo.CurrentCulture);
            MeasuredAmountDisplay = MeasuredAmount.ToString("0.##", CultureInfo.CurrentCulture);
            DifferenceDisplay = (MeasuredAmount - Amount).ToString("0.##", CultureInfo.CurrentCulture);
            Notify(nameof(PricePerKgDisplay));
            Notify(nameof(OriginalAmountDisplay));
            Notify(nameof(MeasuredAmountDisplay));
            Notify(nameof(DifferenceDisplay));
        }

        private void RecalculateMeasuredAmount()
        {
            MeasuredAmount = Math.Round(SPrice * _measuredQty, 2);
            Notify(nameof(MeasuredAmount));
            UpdateDisplays();
        }

        private decimal OrderedQtyDecimal
        {
            get
            {
                return OriginalQty;
            }
        }

        private void Revalidate()
        {
            // No validation message when input is empty
            if (string.IsNullOrWhiteSpace(MeasuredQtyText))
            {
                ValidationMessage = "Quantity cannot be empty.";
                CanSave = false;
                return;
            }

            // Show validation and disable save when measured is greater than ordered qty
            if (_measuredQty > OrderedQtyDecimal)
            {
                ValidationMessage = "Quantity cannot be greater than ordered quantity.";
                CanSave = false;
            }
            else
            {
                ValidationMessage = string.Empty;
                CanSave = true;
            }
        }
    }
}