using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.Logger;
using Techugo.POS.ECOm.Services; // for EditQtyViewModel

namespace Techugo.POS.ECOm.Pages.Dashboard.PickList
{
    public partial class EditPickList : UserControl
    {
        public string[] portList;
        private SerialPort _serialPort;
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

            // Ensure UI reflects whether this is a loose item before any deferred work runs.
            // Hide weigh callout if item is not loose and show an informational message instead.
            if (ItemDetails == null || !ItemDetails.IsLooseItem)
            {
                try
                {
                    WeighCalloutPanel.Visibility = Visibility.Collapsed;
                    NoScaleMessage.Visibility = Visibility.Visible;
                    Message1.Visibility = Visibility.Collapsed;
                    Message2.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    // swallow if controls not available during InitializeComponent sequence
                }
            }
            else
            {
                // for loose items do the usual initialization (deferred to let layout finish)
                Dispatcher.BeginInvoke(new Action(() => InitializeSerialPort()), System.Windows.Threading.DispatcherPriority.Render);
            }

            UpdateKeypadVisibility(false);
        }

        private void InitializeSerialPort()
        {
            // dispose existing
            if (_serialPort != null)
            {
                try
                {
                    if (_serialPort.IsOpen)
                        _serialPort.Close();
                }
                catch { }
                _serialPort.Dispose();
            }


            var portName = App.ServiceProvider?
        .GetService(typeof(Microsoft.Extensions.Options.IOptions<ApiSettings>))
        is Microsoft.Extensions.Options.IOptions<ApiSettings> opt
        ? opt.Value?.WeighingScalePortName
        : null;

         

            _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
            {
                RtsEnable = true,
                DtrEnable = true
            };

            _serialPort.DataReceived += DataReceivedHandler;
           


            try
            {
                _serialPort.Open();
                if (_serialPort.IsOpen)
                {
                    _serialPort.Write("W");
                    LocalFileLogger.Info($"Serial port {_serialPort.PortName} opened.");
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                LocalFileLogger.Info($"Port {portName} is already in use or reserved: {ex.Message}");
                SnackbarService.Enqueue($"Port {portName} is unavailable. Please ensure no other application is using the scale and try again.");
            }
            catch (Exception ex)
            {
                SnackbarService.Enqueue($"Failed to open serial port {portName}. Please check the configuration and ensure the scale is connected.");
                LocalFileLogger.Error("Failed to read WeighingScalePortName from configuration.", ex);
                System.Diagnostics.Debug.WriteLine($"Failed to open serial port: {ex}");
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender is not SerialPort sp)
            {
                LocalFileLogger.Warn("DataReceivedHandler invoked with non-SerialPort sender.");
                return;
            }

            string line;
            try
            {
                // Read exactly once (do not call ReadLine() multiple times).
                line = sp.ReadLine()?.Trim() ?? string.Empty;
                LocalFileLogger.Info($"Serial read on {sp.PortName}: '{line}'");
                LogSerialPortInfo(sp);
            }
            catch (TimeoutException)
            {
                // No complete line available; ignore.
                return;
            }
            catch (Exception ex)
            {
                LocalFileLogger.Error("Failed reading line from serial port.", ex);
                return;
            }

            try
            {
                // Normalize input: keep digits and dot (device uses '.' as decimal separator)
                var cleaned = Regex.Replace(line ?? string.Empty, @"[^0-9.]", "");
                // Remove leading zeros but keep a single zero when appropriate
                cleaned = cleaned.TrimStart('0');
                if (string.IsNullOrEmpty(cleaned) || cleaned.StartsWith("."))
                    cleaned = "0" + cleaned;

                // Parse using invariant culture first (device usually uses '.')
                if (!decimal.TryParse(cleaned, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsed))
                {
                    decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out parsed);
                }

                LocalFileLogger.Info($"Parsed weight '{cleaned}' => {parsed}");

                // Marshal UI and view-model updates to the UI thread
                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        if (ItemDetails != null)
                        {
                            ItemDetails.Weight = parsed;
                            ItemDetails.MeasuredAmount = ItemDetails.EditedQty * ItemDetails.SPrice * parsed;
                            // If ItemDetails implements INotifyPropertyChanged you do NOT need to reassign DataContext
                        }

                        if (MeasuredWeightDisplayTextBox != null)
                            MeasuredWeightDisplayTextBox.Text = parsed.ToString(CultureInfo.CurrentCulture);

                        // This will now always run (unless UI update throws)
                        LocalFileLogger.Info("Inside if (UI updated)");
                    }
                    catch (Exception uiEx)
                    {
                        LocalFileLogger.Error("UI update failed in DataReceivedHandler.", uiEx);
                    }
                });
            }
            catch (Exception ex)
            {
                LocalFileLogger.Error("Error processing serial data.", ex);
            }
        }

        private void LogSerialPortInfo(SerialPort? sp)
        {
            if (sp == null)
            {
                LocalFileLogger.Info("SerialPort: null");
                return;
            }

            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("SerialPort info:");
                sb.AppendLine($"  PortName     : {sp.PortName}");
                sb.AppendLine($"  BaudRate     : {sp.BaudRate}");
                sb.AppendLine($"  Parity       : {sp.Parity}");
                sb.AppendLine($"  DataBits     : {sp.DataBits}");
                sb.AppendLine($"  StopBits     : {sp.StopBits}");
                sb.AppendLine($"  Handshake    : {sp.Handshake}");
                sb.AppendLine($"  RtsEnable    : {sp.RtsEnable}");
                sb.AppendLine($"  DtrEnable    : {sp.DtrEnable}");
                sb.AppendLine($"  Encoding     : {sp.Encoding?.WebName ?? "null"}");
        sb.AppendLine($"  NewLine      : {(sp.NewLine == null ? "(null)" : sp.NewLine.Replace("\r","\\r").Replace("\n","\\n"))}");
        sb.AppendLine($"  IsOpen       : {sp.IsOpen}");
        // BytesToRead/Write may be valid only when open; wrap in try to be safe
        try { sb.AppendLine($"  BytesToRead   : {sp.BytesToRead}"); } catch { sb.AppendLine("  BytesToRead   : (unavailable)"); }
        try { sb.AppendLine($"  BytesToWrite  : {sp.BytesToWrite}"); } catch { sb.AppendLine("  BytesToWrite  : (unavailable)"); }
        try { sb.AppendLine($"  ReadTimeout   : {sp.ReadTimeout}"); } catch { }
        try { sb.AppendLine($"  WriteTimeout  : {sp.WriteTimeout}"); } catch { }

        LocalFileLogger.Info(sb.ToString());
    }
    catch (Exception ex)
    {
        // Logging should not throw — swallow but persist error
        LocalFileLogger.Error("Failed to collect SerialPort info", ex);
            }
}

        private void ModeRadio_Checked(object sender, RoutedEventArgs e) => UpdateKeypadVisibility(true);

        private void UpdateKeypadVisibility(bool isChanged)
        {
            if (PosKeypadPanel == null || WeighCalloutPanel == null) return;

            bool enterQty = RbEnterQty?.IsChecked == true;
            bool checkWeight = RbCheckWeight?.IsChecked == true;
            bool isLoose = ItemDetails?.IsLooseItem == true;

            PosKeypadPanel.Visibility = enterQty ? Visibility.Visible : Visibility.Collapsed;

            // If the item is not loose we hide the weigh panel and show the 'no scale' message
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (checkWeight && isLoose)
                {
                    WeighCalloutPanel.Visibility = Visibility.Visible;
                    NoScaleMessage.Visibility = Visibility.Collapsed;
                    Message1.Visibility = Visibility.Visible;
                    Message2.Visibility = Visibility.Visible;
                }
                else
                {
                    WeighCalloutPanel.Visibility = Visibility.Collapsed;
                    NoScaleMessage.Visibility = Visibility.Visible;
                    Message1.Visibility = Visibility.Collapsed;
                    Message2.Visibility = Visibility.Collapsed;
                }

                try { WeighCalloutPanel.UpdateLayout(); } catch { }
            }), System.Windows.Threading.DispatcherPriority.Render);

            if (enterQty)
            {
                // show quantity in keypad input

            }

            //if (checkWeight)
            //{
            //    if (isChanged && isLoose)
            //    {
            //        CleanupSerialPort();
            //        // Defer LoadComPorts to avoid running before control is fully rendered
            //        Dispatcher.BeginInvoke(new Action(() => InitializeSerialPort()), System.Windows.Threading.DispatcherPriority.Render);
            //    }

            //}
        }


        private void ParseCasWeight(string rawData)
        {
            // Simple regex or substring to extract numbers
            // This is a common pattern for CAS scales
            string trimmed = rawData.Replace(" ", "").Replace("kg", "").Trim();
            if (decimal.TryParse(trimmed, out decimal weight))
            {
                MeasuredQtyTextBox.Text = weight.ToString(CultureInfo.CurrentCulture);
            }
        }

        // --- cleanup when control is unloaded/desposed ---
        private void CleanupSerialPort()
        {
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= DataReceivedHandler;
                try { if (_serialPort.IsOpen) _serialPort.Close(); } catch { }
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        // Ensure the port is closed when the application exits

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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
            CloseClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ItemDetails == null) return;

            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }

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
                    ItemDetails.MeasuredAmount = changedQty * ItemDetails.SPrice;
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

    internal record struct NewStruct(string port, object name, object pnp)
    {
        public static implicit operator (string port, object name, object pnp)(NewStruct value)
        {
            return (value.port, value.name, value.pnp);
        }

        public static implicit operator NewStruct((string port, object name, object pnp) value)
        {
            return new NewStruct(value.port, value.name, value.pnp);
        }
    }
}