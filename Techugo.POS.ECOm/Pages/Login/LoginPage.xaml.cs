using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Services;

namespace Techugo.POS.ECOm.Pages.Login
{
    public partial class LoginPage : UserControl
    {
        public event RoutedEventHandler OtpRequested;
        private readonly ApiService _apiService;
        private readonly SnackbarMessageQueue _messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));

        public LoginPage()
        {
            InitializeComponent();
            SuccessSnackbar.MessageQueue = _messageQueue;

            _apiService = ApiServiceFactory.Create();
            MobileNumberTextBox.PreviewTextInput += MobileNumberTextBox_PreviewTextInput;
            MobileNumberTextBox.TextChanged += MobileNumberTextBox_TextChanged;
            DataObject.AddPastingHandler(MobileNumberTextBox, OnPaste);
            SendOtpButton.IsEnabled = false;
            SendOtpButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
        }
        private void MobileNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits and restrict to 10
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$") || MobileNumberTextBox.Text.Length >= 10;
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pasteText = (string)e.DataObject.GetData(typeof(string));
                if (!Regex.IsMatch(pasteText, @"^\d{1,10}$") || (MobileNumberTextBox.Text.Length + pasteText.Length) > 10)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private async void SendOtpButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new { MobileNo = MobileNumberTextBox.Text };
            try
            {
                BaseResponse result = await _apiService.PostAsync<BaseResponse>("auth/login", data);
                if (result != null)
                {
                    if (result.Success == true)
                    {
                        SnackbarService.Enqueue("OTP sent successfully!");
                        // await Task.Delay(3000); // Wait for 3 seconds
                        OtpRequested?.Invoke(this, new RoutedEventArgs());
                    }
                    else
                    {
                        SnackbarService.Enqueue(result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSuccessSnackbar("Failed to send OTP. Please try again.");
                return;
            } 
        }
        private void MobileNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Remove non-digit characters and trim to 10 digits
            string digits = Regex.Replace(MobileNumberTextBox.Text, @"\D", "");
            if (digits.Length > 10)
                digits = digits.Substring(0, 10);

            if (MobileNumberTextBox.Text != digits)
            {
                int selStart = MobileNumberTextBox.SelectionStart;
                MobileNumberTextBox.Text = digits;
                MobileNumberTextBox.SelectionStart = selStart > digits.Length ? digits.Length : selStart;
            }

            bool isValid = digits.Length == 10;
            SendOtpButton.IsEnabled = isValid;
            SendOtpButton.Background = isValid
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black)
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);

            // Show/hide and update preview text
            if (digits.Length > 0)
            {
                MobilePreviewTextBlock.Visibility = Visibility.Visible;
                MobilePreviewTextBlock.Text = $"Mobile: +91 {digits}";
            }
            else
            {
                MobilePreviewTextBlock.Visibility = Visibility.Collapsed;
                MobilePreviewTextBlock.Text = "";
            }
        }
        private void ShowSuccessSnackbar(string message)
        {
            _messageQueue.Enqueue(message);
        }
        // Expose entered phone number
        public string EnteredPhoneNumber => MobileNumberTextBox.Text;
    }
}
