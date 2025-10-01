using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages.Login
{
    public partial class LoginPage : UserControl
    {
        public event RoutedEventHandler OtpRequested;
        private readonly ApiService _apiService;

        public LoginPage()
        {
            InitializeComponent();
            // Get ApiSettings from DI container
            var apiSettingsOptions = App.ServiceProvider?.GetService(typeof(IOptions<ApiSettings>)) is IOptions<ApiSettings> options ? options : null;
            if (apiSettingsOptions == null)
            {
                throw new System.Exception("ApiSettings not configured.");
            }

            // Use the token stored in TokenService
            _apiService = new ApiService(apiSettingsOptions, TokenService.BearerToken);
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
            BaseResponse result = await _apiService.PostAsync<BaseResponse>("auth/login", data);
            if(result !=null)
            {
                if(result.Success == true)
                {

                    OtpRequested?.Invoke(this, new RoutedEventArgs());
                }
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

        // Expose entered phone number
        public string EnteredPhoneNumber => MobileNumberTextBox.Text;
    }
}
