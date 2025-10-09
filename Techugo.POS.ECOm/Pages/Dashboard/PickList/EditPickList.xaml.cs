using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Techugo.POS.ECom.Model.ViewModel; // for PropertyChangedEventArgs

namespace Techugo.POS.ECOm.Pages.Dashboard.PickList
{
    public partial class EditPickList : UserControl
    {
        public event RoutedEventHandler CloseClicked;
        public event RoutedEventHandler SaveClicked;
        public event PropertyChangedEventHandler PropertyChanged;

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
            // set initial visibility based on default radio
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

            // keep measured textbox in sync with model when switching modes
            if (enterQty)
            {
                MeasuredQtyTextBox.Text = ItemDetails?.MeasuredQty.ToString() ?? string.Empty;
            }
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

            // parse measured quantity (integer). adjust parsing if decimals are needed.
            if (int.TryParse(MeasuredQtyTextBox.Text, out int measured))
            {
                ItemDetails.MeasuredQty = measured;
                // optionally compute MeasuredAmount and OriginalAmount here and call UpdateDisplays
                ItemDetails.UpdateDisplays();
            }

            SaveClicked?.Invoke(this, new RoutedEventArgs());
        }
    }
}