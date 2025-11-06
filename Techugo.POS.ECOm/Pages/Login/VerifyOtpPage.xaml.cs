using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Services;

namespace Techugo.POS.ECOm.Pages.Login
{
    public partial class VerifyOtpPage : UserControl
    {
        public event RoutedEventHandler OtpVerified;
        private readonly ApiService _apiService;

        public string PhoneNumber { get; set; }
        public string PhoneNumberWithoutCode { get; set; }

        // resend timer
        private DispatcherTimer _resendTimer;
        private int _resendSecondsRemaining = 0;
        private const int RESEND_INTERVAL_SECONDS = 30;
        private bool _isCountdownActive = false;

        public VerifyOtpPage(string phoneNumber)
        {
            InitializeComponent();
            PhoneNumber = "+91 " + phoneNumber;
            PhoneNumberWithoutCode = phoneNumber;
            DataContext = this; // For simple binding
            _apiService = ApiServiceFactory.Create();

            // initialize resend timer
            _resendTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _resendTimer.Tick += ResendTimer_Tick;

            // start initial countdown when page opens
            StartResendCountdown();
        }

        private void StartResendCountdown()
        {
            _resendSecondsRemaining = RESEND_INTERVAL_SECONDS;
            _isCountdownActive = true;

            // disable button while countdown running (non-clickable) and update text
            ResendButton.IsEnabled = false;
            UpdateResendUi();

            _resendTimer.Stop();
            _resendTimer.Start();
        }

        private void ResendTimer_Tick(object sender, EventArgs e)
        {
            _resendSecondsRemaining--;
            if (_resendSecondsRemaining <= 0)
            {
                _resendTimer.Stop();
                _isCountdownActive = false;

                // Restore clickable state and label
                ResendButton.IsEnabled = true;
                ResendLabelRun.Text = "Resend OTP";
                ResendSpacerRun.Text = "";
                ResendTimerRun.Text = "";
            }
            else
            {
                UpdateResendUi();
            }
        }

        private void UpdateResendUi()
        {
            var ts = TimeSpan.FromSeconds(Math.Max(0, _resendSecondsRemaining));
            // show "Resend OTP in 0:09" with timer colored
            ResendLabelRun.Text = "Resend OTP";
            ResendSpacerRun.Text = " in ";
            ResendTimerRun.Text = $"{ts.Minutes}:{ts.Seconds:D2}";
        }

        private async void ResendButton_Click(object sender, RoutedEventArgs e)
        {
            // defensive: if countdown active, ignore clicks (button should be disabled)
            if (_isCountdownActive) return;

            // show sending state
            ResendLabelRun.Text = "Sending...";
            ResendSpacerRun.Text = "";
            ResendTimerRun.Text = "";

            try
            {
                var payload = new { MobileNo = PhoneNumberWithoutCode };

                // Replace with correct endpoint if different
                BaseResponse response = await _apiService.PostAsync<BaseResponse>("auth/login", payload);

                if (response != null && response.Success)
                {
                    // start countdown and make button non-clickable
                    StartResendCountdown();
                    SnackbarService.Enqueue("OTP sent successfully.");
                }
                else
                {
                    // restore label on failure, keep clickable
                    ResendLabelRun.Text = "Resend OTP";
                    ResendSpacerRun.Text = "";
                    ResendTimerRun.Text = "";
                    SnackbarService.Enqueue("Failed to send OTP.");
                }
            }
            catch (Exception ex)
            {
                // restore label on failure
                ResendLabelRun.Text = "Resend OTP";
                ResendSpacerRun.Text = "";
                ResendTimerRun.Text = "";
                SnackbarService.Enqueue($"Failed to send OTP: {ex.Message}");
            }
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
                try
                {
                    OTPVerifiedResponse result = await _apiService.PostAsync<OTPVerifiedResponse>("auth/verify-otp", data);
                    if (result != null && result.Success == true)
                    {
                        TokenService.BearerToken = result.Data.Token;
                        OtpVerified?.Invoke(this, new RoutedEventArgs());
                    }
                    else
                    {
                        SnackbarService.Enqueue(result?.Message);
                    }
                }
                catch
                {
                    SnackbarService.Enqueue("Something went wrong");
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
                mainWindow.GetType().GetMethod("ShowLogin", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.Invoke(mainWindow, null);
            }
        }

        private bool IsAllDigits(string s) => s.All(char.IsDigit);
    }
}
