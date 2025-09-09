using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Techugo.POS.ECOm.Pages;

namespace Techugo.POS.ECOm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DashboardPage dashboardPage;

        public MainWindow()
        {
            InitializeComponent();
            dashboardPage = new DashboardPage();
            dashboardPage.TotalOrdersClicked += DashboardPage_TotalOrdersClicked;
            dashboardPage.PickListClicked += DashboardPage_PickListClicked;
            // Wire up other tile events here
            DashboardContent.Content = dashboardPage;
        }

       

        private void DashboardPage_TotalOrdersClicked(object sender, RoutedEventArgs e)
        {
            var page = new TotalOrdersPage();
            page.BackRequested += (s, args) => DashboardContent.Content = dashboardPage;
            DashboardContent.Content = page;
        }
        private void DashboardPage_PickListClicked(object sender, RoutedEventArgs e)
        {
            var page = new PickListPage();
            page.BackRequested += (s, args) => DashboardContent.Content = dashboardPage;
            DashboardContent.Content = page;
        }

        // Repeat for other tiles:
        // private void DashboardPage_PickListClicked(object sender, RoutedEventArgs e) { ... }
        // etc.
    }
}