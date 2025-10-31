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

            if (ItemDetails != null)
            {
                // Ensure initial measured amount equals original amount (if not provided)
                if (ItemDetails.MeasuredAmount == 0m)
                    ItemDetails.MeasuredAmount = ItemDetails.Amount;

                // If MeasuredQty is not set, try to derive from OrderedQty (optional)
                if (ItemDetails.MeasuredQty == 0m && decimal.TryParse(ItemDetails.OrderedQty, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
                    ItemDetails.MeasuredQty = parsed;

                ItemDetails.UpdateDisplays();

                //if (MeasuredWeightDisplayTextBox != null)
                //    MeasuredWeightDisplayTextBox.Text = ItemDetails.MeasuredWeight ?? string.Empty;
            }

            // initialize measured input UI to show quantity (MeasuredQty), not weight
            MeasuredQtyTextBox.Text = ItemDetails != null ? GetInitialInputText() : string.Empty;
            UpdateKeypadVisibility();
        }

        private string GetInitialInputText()
        {
            // Bind keypad input to quantity; use MeasuredQty first, fallback to numeric part of MeasuredWeight
            if (ItemDetails != null && ItemDetails.MeasuredQty != 0m)
                return ItemDetails.MeasuredQty.ToString(CultureInfo.CurrentCulture);

            if (!string.IsNullOrWhiteSpace(ItemDetails?.MeasuredWeight))
            {
                var s = ItemDetails.MeasuredWeight.Trim();
                if (s.EndsWith("kg", StringComparison.OrdinalIgnoreCase))
                    s = s.Substring(0, s.Length - 2).Trim();
                return s;
            }

            return string.Empty;
        }

        private void ModeRadio_Checked(object sender, RoutedEventArgs e) => UpdateKeypadVisibility();

        private void UpdateKeypadVisibility()
        {
            if (PosKeypadPanel == null || WeighCalloutPanel == null) return;

            bool enterQty = RbEnterQty?.IsChecked == true;
            bool checkWeight = RbCheckWeight?.IsChecked == true;

            PosKeypadPanel.Visibility = enterQty ? Visibility.Visible : Visibility.Collapsed;
            WeighCalloutPanel.Visibility = checkWeight ? Visibility.Visible : Visibility.Collapsed;

            if (enterQty)
            {
                // show quantity in keypad input
                MeasuredQtyTextBox.Text = ItemDetails?.MeasuredQty.ToString(CultureInfo.CurrentCulture) ?? string.Empty;

                if (ItemDetails != null && decimal.TryParse(MeasuredQtyTextBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal qty))
                {
                    // measured amount = SPrice * quantity
                    ItemDetails.MeasuredAmount = Math.Round(ItemDetails.SPrice * qty, 2);
                    ItemDetails.MeasuredQty = qty;
                    ItemDetails.UpdateDisplays();
                }

                //if (MeasuredWeightDisplayTextBox != null)
                //    MeasuredWeightDisplayTextBox.Text = ItemDetails.MeasuredWeight ?? string.Empty;
            }

            if (checkWeight)
            {
                // Random weight functionality is disabled for now.
                // If you need to re-enable later, call GenerateRandomMeasuredWeight().
            }
        }

        private void GenerateRandomMeasuredWeight()
        {
            // Random weight generation is disabled temporarily.
            // Intentionally left empty so no random weight is produced.
            // If you want to re-enable, restore the previous implementation here.
            return;
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

            var decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (tag == "BACK")
            {
                var txt = MeasuredQtyTextBox.Text;
                if (txt.Length > 0)
                    MeasuredQtyTextBox.Text = txt.Substring(0, txt.Length - 1);
            }
            else if (tag == "CLEAR")
            {
                if (!MeasuredQtyTextBox.Text.Contains(decSep))
                {
                    if (string.IsNullOrEmpty(MeasuredQtyTextBox.Text))
                        MeasuredQtyTextBox.Text = "0" + decSep;
                    else
                        MeasuredQtyTextBox.Text += decSep;
                }
            }
            else
            {
                MeasuredQtyTextBox.Text += tag;
            }

            // When in Enter Measured Qty mode, interpret keypad input as quantity (not weight)
            if (RbEnterQty?.IsChecked == true && ItemDetails != null)
            {
                if (TryParseInputAsDecimal(MeasuredQtyTextBox.Text, out decimal measuredQty))
                {
                    // update quantity and measured amount (SPrice * quantity)
                    ItemDetails.MeasuredQty = measuredQty;
                    ItemDetails.MeasuredAmount = Math.Round(ItemDetails.SPrice * measuredQty, 2);

                    ItemDetails.UpdateDisplays();

                    // do not modify MeasuredWeight here since MeasuredQty is units
                }
                else
                {
                    // invalid input — keep previous measured amount/qty
                    ItemDetails.UpdateDisplays();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => CloseClicked?.Invoke(this, new RoutedEventArgs());

        private void Cancel_Click(object sender, RoutedEventArgs e) => CloseClicked?.Invoke(this, new RoutedEventArgs());

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ItemDetails == null) return;

            if (RbEnterQty?.IsChecked == true)
            {
                if (TryParseInputAsDecimal(MeasuredQtyTextBox.Text, out decimal measured))
                {
                    ItemDetails.MeasuredQty = measured;
                    ItemDetails.MeasuredAmount = Math.Round(ItemDetails.SPrice * measured, 2);
                }
            }

            ItemDetails.UpdateDisplays();

            //if (MeasuredWeightDisplayTextBox != null)
            //    MeasuredWeightDisplayTextBox.Text = ItemDetails.MeasuredWeight;

            SaveClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}