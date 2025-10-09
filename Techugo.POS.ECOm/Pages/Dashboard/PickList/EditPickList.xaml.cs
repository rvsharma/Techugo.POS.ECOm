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
            MeasuredQtyTextBox.Text = ItemDetails?.MeasuredQty.ToString() ?? string.Empty;
            // set initial visibility and initial values based on default radio
            UpdateKeypadVisibility();
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
                // keep measured textbox in sync with model when switching modes
                MeasuredQtyTextBox.Text = ItemDetails?.MeasuredQty.ToString() ?? string.Empty;
                // ensure MeasuredAmount reflects current MeasuredQty
                if (ItemDetails != null)
                {
                    ItemDetails.MeasuredAmount = ItemDetails.PricePerKg * ItemDetails.MeasuredQty;
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

            // store measured weight as string (with unit if desired)
            ItemDetails.MeasuredWeight = string.Format(CultureInfo.CurrentCulture, "{0:N2} kg", weight);

            // update measured amount using price per kg
            ItemDetails.MeasuredAmount = Math.Round(ItemDetails.PricePerKg * weight, 2);
            ItemDetails.UpdateDisplays();
        }

        private void KeypadButton_Click(object sender, RoutedEventArgs e)
        {
            if (MeasuredQtyTextBox == null) return;
            if (sender is not Button btn) return;
            var tag = (btn.Tag ?? string.Empty).ToString();

            if (tag == "BACK")
            {
                var txt = MeasuredQtyTextBox.Text;
                if (txt.Length > 0)
                    MeasuredQtyTextBox.Text = txt.Substring(0, txt.Length - 1);
            }
            else if (tag == "CLEAR")
            {
                MeasuredQtyTextBox.Text = string.Empty;
            }
            else
            {
                // Append digit (prevent leading zeros if desired)
                MeasuredQtyTextBox.Text += tag;
            }

            // Live-update the numeric model when in Enter Measured Qty mode
            if (RbEnterQty?.IsChecked == true && ItemDetails != null)
            {
                if (int.TryParse(MeasuredQtyTextBox.Text, out int measuredInt))
                {
                    ItemDetails.MeasuredQty = measuredInt;
                    ItemDetails.MeasuredAmount = Math.Round(ItemDetails.PricePerKg * measuredInt, 2);
                    ItemDetails.UpdateDisplays();
                }
                else
                {
                    // clear measured qty if textbox not parseable
                    ItemDetails.MeasuredQty = 0;
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

            // If Enter Measured Qty mode, parse and persist MeasuredQty (already updated live)
            if (RbEnterQty?.IsChecked == true)
            {
                if (int.TryParse(MeasuredQtyTextBox.Text, out int measured))
                {
                    ItemDetails.MeasuredQty = measured;
                    ItemDetails.MeasuredAmount = Math.Round(ItemDetails.PricePerKg * measured, 2);
                    ItemDetails.UpdateDisplays();
                }
            }

            // If Check Weight mode, MeasuredWeight and MeasuredAmount are already set by GenerateRandomMeasuredWeight

            SaveClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}