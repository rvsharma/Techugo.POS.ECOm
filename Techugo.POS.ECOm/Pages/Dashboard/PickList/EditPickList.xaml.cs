using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model.ViewModel; // for EditQtyViewModel

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
            UpdateKeypadVisibility(false);
            //LoadComPorts();
        }

        private void LoadComPorts()
        {
            portList = SerialPort.GetPortNames();

            // try to find USB-backed COM ports via WMI (Win32_PnPEntity contains COM name in the 'Name' property)
            var usbPorts = GetUsbSerialPorts();
            if (usbPorts.Any())
            {
                // pick the first USB serial device (or apply your own heuristics)
                var portToUse = usbPorts.First().PortName;
                InitializeSerialPort(portToUse);
                Debug.WriteLine($"Auto-selected USB serial port: {portToUse}");
                return;
            }

            // fallback: if none identified, use first available COM or COM5 default
            if (portList != null && portList.Length > 0)
                InitializeSerialPort(portList[0]);
            else
                InitializeSerialPort("COM6");


        }
        /// <summary>
        /// Returns detected serial ports backed by USB devices.
        /// Each tuple contains (PortName, Description, PnpDeviceId).
        /// Requires System.Management (Windows).
        /// </summary>
        private List<(string PortName, string Description, string PnpDeviceId)> GetUsbSerialPorts()
        {
            var list = new List<(string, string, string)>();
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");
                foreach (ManagementObject mo in searcher.Get())
                {
                    var name = (mo["Name"] as string) ?? string.Empty;         // e.g. "USB-SERIAL CH340 (COM3)"
                    var pnp = (mo["PNPDeviceID"] as string) ?? string.Empty;   // contains vendor/usb info on USB devices
                                                                               // extract COMx from the Name property
                    var m = Regex.Match(name, @"\((COM\d+)\)");
                    if (!m.Success) continue;
                    var port = m.Groups[1].Value;

                    // keep entries where PNPDeviceID or Name mentions "USB" (case-insensitive)
                    if (name.IndexOf("USB", StringComparison.OrdinalIgnoreCase) >= 0 &&
                        name.IndexOf("Serial", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        list.Add((port, name, pnp));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetUsbSerialPorts failed: {ex.Message}");
            }
            return list;
        }

        private void InitializeSerialPort(string portName = "COM6")
        {
            // dispose existing
            try { _serialPort?.Dispose(); } catch { }

            _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            _serialPort.RtsEnable = true; // Essential for many serial adapters
            _serialPort.DtrEnable = true; // Signals the PC is ready to receive
            _serialPort.DataReceived += DataReceivedHandler;


            try
            {
                _serialPort.Open();
                _serialPort.Write("W");
                System.Diagnostics.Debug.WriteLine($"Serial port {_serialPort.PortName} opened.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open serial port: {ex}");
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            if (sp != null)
            {
                string data = sp.ReadLine().Trim();

                try
                {
                    string cleaned = Regex.Replace(data, @"[^0-9.]", "");

                    // Remove leading zeros (but keep "0" if it's the only digit)
                    cleaned = cleaned.TrimStart('0');
                    if (string.IsNullOrEmpty(cleaned) || cleaned.StartsWith("."))
                        cleaned = "0" + cleaned;
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (ItemDetails != null)
                        {
                            ItemDetails.Weight = decimal.Parse(cleaned);
                            ItemDetails.MeasuredAmount = ItemDetails.EditedQty * ItemDetails.SPrice * decimal.Parse(cleaned);

                            DataContext = ItemDetails; 
                        }

                        if (MeasuredWeightDisplayTextBox != null)
                            MeasuredWeightDisplayTextBox.Text = decimal.Parse(cleaned).ToString(CultureInfo.CurrentCulture);
                    });

                    
                    //DataContext = ItemDetails;
                    // Parse the weight data here (usually a string ending in 'kg' or 'lb')
                    //Console.WriteLine("Weight Received: " + data);
                }
                catch (Exception)
                {

                    throw;
                }
                
            }
        }


        private void ModeRadio_Checked(object sender, RoutedEventArgs e) => UpdateKeypadVisibility(true);

        private void UpdateKeypadVisibility(bool isChanged)
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
                if(isChanged)
                {
                    CleanupSerialPort();
                    LoadComPorts();
                }
                
            }
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