using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Techugo.POS.ECOm.Pages.Login
{
    public partial class VerifyOtpPage : UserControl
    {
        public event RoutedEventHandler OtpVerified;

        public VerifyOtpPage()
        {
            InitializeComponent();
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming your TextBoxes are named OtpBox1, OtpBox2, ..., OtpBox6 in XAML
            string otp = $"{OtpBox1.Text}{OtpBox2.Text}{OtpBox3.Text}{OtpBox4.Text}{OtpBox5.Text}{OtpBox6.Text}";

            // Demo logic: accept any 6 digits or "123456"
            if (otp.Length == 6 && (otp == "123456" || IsAllDigits(otp)))
            {
                OtpVerified?.Invoke(this, new RoutedEventArgs());
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please enter a valid 6-digit code.", "OTP Verification", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool IsAllDigits(string s) => s.All(char.IsDigit);

    }
}
