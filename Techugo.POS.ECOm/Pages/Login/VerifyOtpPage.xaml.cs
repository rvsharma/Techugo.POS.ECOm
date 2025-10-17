using Microsoft.Extensions.Options;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages.Login
{
    public partial class VerifyOtpPage : UserControl
    {
        public event RoutedEventHandler OtpVerified;
        private readonly ApiService _apiService;

        public string PhoneNumber { get; set; }
        public string PhoneNumberWithoutCode { get; set; }

        public VerifyOtpPage(string phoneNumber)
        {
            InitializeComponent();
            PhoneNumber = "+91 " + phoneNumber;
            PhoneNumberWithoutCode = phoneNumber;
            DataContext = this; // For simple binding
            _apiService = ApiServiceFactory.Create();
        }
        private void OtpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null) return;
            if (box.Text.Length == 1)
            {
                MoveToNextBox(box);
            }
            bool allFilled = !string.IsNullOrEmpty(OtpBox1.Text)
        && !string.IsNullOrEmpty(OtpBox2.Text)
        && !string.IsNullOrEmpty(OtpBox3.Text)
        && !string.IsNullOrEmpty(OtpBox4.Text)
        && !string.IsNullOrEmpty(OtpBox5.Text)
        && !string.IsNullOrEmpty(OtpBox6.Text);

            VerifyButton.Background = allFilled
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black)
                : new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7F7B81"));
        
        }

        private void OtpBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null) return;

            if (e.Key == Key.Back && string.IsNullOrEmpty(box.Text))
            {
                MoveToPreviousBox(box);
            }
        }

        private void MoveToNextBox(TextBox currentBox)
        {
            if (currentBox == OtpBox1) OtpBox2.Focus();
            else if (currentBox == OtpBox2) OtpBox3.Focus();
            else if (currentBox == OtpBox3) OtpBox4.Focus();
            else if (currentBox == OtpBox4) OtpBox5.Focus();
            else if (currentBox == OtpBox5) OtpBox6.Focus();
        }

        private void MoveToPreviousBox(TextBox currentBox)
        {
            if (currentBox == OtpBox6) OtpBox5.Focus();
            else if (currentBox == OtpBox5) OtpBox4.Focus();
            else if (currentBox == OtpBox4) OtpBox3.Focus();
            else if (currentBox == OtpBox3) OtpBox2.Focus();
            else if (currentBox == OtpBox2) OtpBox1.Focus();
        }

        private async void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            string otp = $"{OtpBox1.Text}{OtpBox2.Text}{OtpBox3.Text}{OtpBox4.Text}{OtpBox5.Text}{OtpBox6.Text}";

            if (otp == "123456")
            {
                var data = new { MobileNo = PhoneNumberWithoutCode, OTP = otp };
                //var data = new { MobileNo = "9917000000", OTP = otp };
                try
                {
                    OTPVerifiedResponse result = await _apiService.PostAsync<OTPVerifiedResponse>("auth/verify-otp", data);
                    if (result != null)
                    {
                        if (result.Success == true)
                        {
                            TokenService.BearerToken = result.Data.Token;
                            OtpVerified?.Invoke(this, new RoutedEventArgs());
                        }
                    }

                }
                catch {
                    // OtpVerified?.Invoke(this, new RoutedEventArgs());
                }   
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please enter a valid 6-digit code.", "OTP Verification", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as Techugo.POS.ECOm.MainWindow;
            if (mainWindow != null)
            {
                // Use the navigation method to ensure events are wired
                mainWindow.GetType().GetMethod("ShowLogin", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.Invoke(mainWindow, null);
            }
        }

        private bool IsAllDigits(string s) => s.All(char.IsDigit);

    }
}
