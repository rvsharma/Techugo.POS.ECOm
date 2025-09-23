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
            OtpVerified?.Invoke(this, new RoutedEventArgs());
        }
    }
}
