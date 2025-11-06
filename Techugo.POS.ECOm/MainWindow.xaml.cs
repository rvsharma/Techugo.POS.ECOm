using System.Windows;
using Techugo.POS.ECOm.Pages.Login;
using Techugo.POS.ECOm.Pages.Dashboard;
using Techugo.POS.ECOm.Pages;

namespace Techugo.POS.ECOm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginPage loginPage; // Store reference to LoginPage

        public MainWindow()
        {
            InitializeComponent();
            ShowLogin();
        }

        private void ShowLogin()
        {
            loginPage = new LoginPage();
            loginPage.OtpRequested += (s, e) => ShowVerifyOtp();
            MainContent.Content = loginPage;
        }

        private void ShowVerifyOtp()
        {
            string phoneNumber = loginPage?.EnteredPhoneNumber ?? "";
            var verifyOtpPage = new VerifyOtpPage(phoneNumber);
            verifyOtpPage.OtpVerified += (s, e) => ShowLayoutPage();
            MainContent.Content = verifyOtpPage;
        }

        private void ShowLayoutPage()
        {
            var layoutPage = new LayoutPage();
            MainContent.Content = layoutPage;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
        }

        // Optional: allow user to exit fullscreen with Esc (restore caption and allow resizing)
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Topmost = false;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                WindowState = WindowState.Normal;
            }
        }
    }
}