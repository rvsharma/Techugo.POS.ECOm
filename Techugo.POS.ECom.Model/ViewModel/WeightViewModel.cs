using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model.ViewModel
{
    public class WeightViewModel : INotifyPropertyChanged
    {
        private SerialPort _serialPort;
        private CancellationTokenSource _cts;

        private string _detectedSettings;
        private string _lastRaw;
        private string _lastError;

        // Public diagnostic properties
        public string DetectedSettings
        {
            get => _detectedSettings;
            private set { _detectedSettings = value; OnPropertyChanged(nameof(DetectedSettings)); }
        }

        public string LastRaw
        {
            get => _lastRaw;
            private set { _lastRaw = value; OnPropertyChanged(nameof(LastRaw)); }
        }

        public string LastError
        {
            get => _lastError;
            private set { _lastError = value; OnPropertyChanged(nameof(LastError)); }
        }

        private string _weight;
        public string Weight
        {
            get => _weight;
            set { _weight = value; OnPropertyChanged(nameof(Weight)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void StartSerial(string portName = "COM6", int baudRate = 2400)
        {
            StopSerial(); // clean up if already running

            _serialPort = new SerialPort(portName)
            {
                BaudRate = baudRate,          // check scale manual
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                RtsEnable = false,        // many adapters require these
                DtrEnable = false,
                NewLine = "\r\n"
            };


            try
            {
                _serialPort.Open();
                System.Diagnostics.Debug.WriteLine($"Serial port {_serialPort.PortName} opened at {baudRate}bps.");
                // send a terminated request line — many scales expect CR/LF
                try { _serialPort.Write("P\r\n"); } catch { _serialPort.Write("P"); }
                _cts = new CancellationTokenSource();

                Task.Run(() => ReadLoop(_cts.Token));
            }
            catch (Exception ex)
            {
                Weight = $"Failed to open port: {ex.Message}";
            }
        }

        private async Task ReadLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        string line = _serialPort.ReadLine().Trim();
                        System.Diagnostics.Debug.WriteLine($"Serial read: {line}");

                        // Example parsing: strip "kg" or "lb"
                        string parsed = line;
                        if (parsed.EndsWith("kg", StringComparison.OrdinalIgnoreCase))
                            parsed = parsed.Replace("kg", "").Trim();
                        else if (parsed.EndsWith("lb", StringComparison.OrdinalIgnoreCase))
                            parsed = parsed.Replace("lb", "").Trim();

                        Weight = parsed;
                    }
                }
                catch (TimeoutException) { /* ignore */ }
                catch (Exception ex)
                {
                    Weight = $"Error: {ex.Message}";
                }

                await Task.Delay(100); // small pause to avoid busy loop
            }
        }

        public void StopSerial()
        {
            try
            {
                _cts?.Cancel();
                if (_serialPort != null)
                {
                    if (_serialPort.IsOpen) _serialPort.Close();
                    _serialPort.Dispose();
                }
            }
            catch { }
        }
    }
}
