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
            UpdateKeypadVisibility();
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
                
                }
                else
                {
                    // invalid input — keep previous measured amount/qty
                    
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => CloseClicked?.Invoke(this, new RoutedEventArgs());

        private void Cancel_Click(object sender, RoutedEventArgs e) => CloseClicked?.Invoke(this, new RoutedEventArgs());

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ItemDetails == null) return;


            //if (MeasuredWeightDisplayTextBox != null)
            //    MeasuredWeightDisplayTextBox.Text = ItemDetails.MeasuredWeight;

            SaveClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void MeasuredQtyManualTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (int.TryParse(textBox.Text, out int changedQty))
                {
                    ItemDetails.MeasuredAmount = changedQty * ItemDetails.Amount;
                    if(changedQty > ItemDetails.OrderedQty)
                    {
                        ItemDetails.CanSave = false;
                        ItemDetails.ValidationMessage = "Qty cannot be more than ordered.";
                    }
                    else if(changedQty < 0)
                    {
                        ItemDetails.CanSave = false;
                    }
                    else
                    {
                        ItemDetails.CanSave = true;
                        ItemDetails.ValidationMessage = "";
                    }
                    DataContext = ItemDetails;
                }
                else
                {
                    ItemDetails.CanSave = false;
                    ItemDetails.ValidationMessage = "Qty cannot empty.";
                    DataContext = ItemDetails;
                }
            }
        }

    }
}