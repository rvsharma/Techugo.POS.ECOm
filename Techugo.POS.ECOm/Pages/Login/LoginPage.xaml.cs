using System.Windows;
using System.Windows.Controls;

namespace Techugo.POS.ECOm.Pages.Login
{
    public partial class LoginPage : UserControl
    {
        public event RoutedEventHandler OtpRequested;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void SendOtpButton_Click(object sender, RoutedEventArgs e)
        {
            OtpRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}
