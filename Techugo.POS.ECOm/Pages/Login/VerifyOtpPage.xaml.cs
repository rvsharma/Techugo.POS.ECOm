using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Techugo.POS.ECOm.Pages.Login
{
    public partial class VerifyOtpPage : UserControl
    {
        public event RoutedEventHandler OtpVerified;

        public string PhoneNumber { get; set; }

        public VerifyOtpPage(string phoneNumber)
        {
            InitializeComponent();
            PhoneNumber = phoneNumber;
            DataContext = this; // For simple binding
        }
        private void OtpBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null) return;
            if (box.Text.Length == 1)
            {
                MoveToNextBox(box);
            }
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

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming your TextBoxes are named OtpBox1, OtpBox2, ..., OtpBox6 in XAML
            string otp = $"{OtpBox1.Text}{OtpBox2.Text}{OtpBox3.Text}{OtpBox4.Text}{OtpBox5.Text}{OtpBox6.Text}";

            // Demo logic: accept any 6 digits or "123456"
            if (otp == "123456")
            {
                OtpVerified?.Invoke(this, new RoutedEventArgs());
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
