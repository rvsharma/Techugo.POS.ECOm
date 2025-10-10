using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;
using Techugo.POS.ECom.Model.ViewModel; // for EditQtyViewModel

namespace Techugo.POS.ECOm.Pages.Dashboard.PickList
{
    public partial class EditPickList : UserControl
    {
        public event RoutedEventHandler CloseClicked;
        public event RoutedEventHandler SaveClicked;
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly Random _random = new();
        private EditQtyViewModel _itemDetails;
        public EditQtyViewModel ItemDetails
        {
            get => _itemDetails;
            set
            {
                _itemDetails = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemDetails)));
            }
        }

        public EditPickList(EditQtyViewModel itemDetails)
        {
            InitializeComponent();
            ItemDetails = itemDetails;
            DataContext = ItemDetails;

            // initialize measured input UI
            MeasuredQtyTextBox.Text = ItemDetails != null ? GetInitialInputText() : string.Empty;
            // set initial visibility and initial values based on default radio
            UpdateKeypadVisibility();
        }

        private string GetInitialInputText()
        {
            // If a measured weight exists like "1.23 kg", return numeric part; otherwise fall back to MeasuredQty
            if (!string.IsNullOrWhiteSpace(ItemDetails?.MeasuredWeight))
            {
                // remove non-numeric trailing unit and parse using current culture
                var s = ItemDetails.MeasuredWeight.Trim();
                // remove "kg" (case-insensitive) and whitespace
                if (s.EndsWith("kg", StringComparison.OrdinalIgnoreCase))
                    s = s.Substring(0, s.Length - 2).Trim();
                return s;
            }

            return ItemDetails?.MeasuredQty.ToString() ?? string.Empty;
        }

        private void ModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            UpdateKeypadVisibility();
        }

        private void UpdateKeypadVisibility()
        {
            if (PosKeypadPanel == null || WeighCalloutPanel == null) return;

            bool enterQty = RbEnterQty?.IsChecked == true;
            bool checkWeight = RbCheckWeight?.IsChecked == true;

            PosKeypadPanel.Visibility = enterQty ? Visibility.Visible : Visibility.Collapsed;
            WeighCalloutPanel.Visibility = checkWeight ? Visibility.Visible : Visibility.Collapsed;

            if (enterQty)
            {
                // initialize input textbox from existing measured weight (numeric only)
                MeasuredQtyTextBox.Text = GetInitialInputText();

                // ensure MeasuredAmount reflects current MeasuredQty/MeasuredWeight
                if (ItemDetails != null)
                {
                    // try parse MeasuredWeight numeric part
                    if (TryParseInputAsDecimal(MeasuredQtyTextBox.Text, out decimal wt))
                    {
                        ItemDetails.MeasuredWeight = string.Format(CultureInfo.CurrentCulture, "{0:N2} kg", wt);
                        ItemDetails.MeasuredAmount = Math.Round(ItemDetails.PricePerKg * wt, 2);
                    }
                    else
                    {
                        // fallback to existing values
                        // keep existing ItemDetails.MeasuredWeight / MeasuredAmount
                    }
                    ItemDetails.UpdateDisplays();
                }
            }

            if (checkWeight)
            {
                // generate a random decimal weight and update the model
                GenerateRandomMeasuredWeight();
            }
        }

        private void GenerateRandomMeasuredWeight()
        {
            if (ItemDetails == null) return;

            // Example: random weight between 0.10 and 2.50 with 2 decimals
            double min = 0.10;
            double max = 2.50;
            double val = _random.NextDouble() * (max - min) + min;
            decimal weight = Math.Round((decimal)val, 2);

            // store measured weight as string (with unit)
            ItemDetails.MeasuredWeight = string.Format(CultureInfo.CurrentCulture, "{0:N2} kg", weight);

            // update measured amount using price per kg
            ItemDetails.MeasuredAmount = Math.Round(ItemDetails.PricePerKg * weight, 2);
            ItemDetails.UpdateDisplays();

            // also reflect numeric value in input textbox when showing keypad
            MeasuredQtyTextBox.Text = weight.ToString(CultureInfo.CurrentCulture);
        }

        private bool TryParseInputAsDecimal(string input, out decimal value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            return decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
        }

        private void KeypadButton_Click(object sender, RoutedEventArgs e)
        {
            if (MeasuredQtyTextBox == null) return;
            if (sender is not Button btn) return;
            var tag = (btn.Tag ?? string.Empty).ToString();

            // Use culture-specific decimal separator
            var decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (tag == "BACK")
            {
                var txt = MeasuredQtyTextBox.Text;
                if (txt.Length > 0)
                    MeasuredQtyTextBox.Text = txt.Substring(0, txt.Length - 1);
            }
            else if (tag == "CLEAR")
            {
                // act as decimal separator: append if not already present
                if (!MeasuredQtyTextBox.Text.Contains(decSep))
                {
                    if (string.IsNullOrEmpty(MeasuredQtyTextBox.Text))
                        MeasuredQtyTextBox.Text = "0" + decSep; // prepend zero for clarity
                    else
                        MeasuredQtyTextBox.Text += decSep;
                }
            }
            else
            {
                // Append digit (prevent leading zeros if desired)
                MeasuredQtyTextBox.Text += tag;
            }

            // Live-update the numeric model when in Enter Measured Qty mode
            if (RbEnterQty?.IsChecked == true && ItemDetails != null)
            {
                if (TryParseInputAsDecimal(MeasuredQtyTextBox.Text, out decimal measuredDecimal))
                {
                    // update MeasuredWeight display (string) and MeasuredAmount (decimal)
                    ItemDetails.MeasuredWeight = string.Format(CultureInfo.CurrentCulture, "{0:N2} kg", measuredDecimal);
                    ItemDetails.MeasuredAmount = Math.Round(ItemDetails.PricePerKg * measuredDecimal, 2);

                    // optionally update integer MeasuredQty if value is whole number
                    if (decimal.Truncate(measuredDecimal) == measuredDecimal)
                        ItemDetails.MeasuredQty = (int)measuredDecimal;

                    ItemDetails.UpdateDisplays();
                }
                else
                {
                    // not parseable; clear numeric model but keep typed input
                    ItemDetails.MeasuredAmount = 0;
                    ItemDetails.UpdateDisplays();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ItemDetails == null) return;

            // If Enter Measured Qty mode, parse and persist MeasuredWeight and MeasuredAmount
            if (RbEnterQty?.IsChecked == true)
            {
                if (TryParseInputAsDecimal(MeasuredQtyTextBox.Text, out decimal measured))
                {
                    ItemDetails.MeasuredWeight = string.Format(CultureInfo.CurrentCulture, "{0:N2} kg", measured);
                    ItemDetails.MeasuredAmount = Math.Round(ItemDetails.PricePerKg * measured, 2);

                    // if whole number, update MeasuredQty as integer
                    if (decimal.Truncate(measured) == measured)
                        ItemDetails.MeasuredQty = (int)measured;
                }
            }

            // If Check Weight mode, MeasuredWeight and MeasuredAmount are already set by GenerateRandomMeasuredWeight

            ItemDetails.UpdateDisplays();
            SaveClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}