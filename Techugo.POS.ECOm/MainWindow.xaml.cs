using System.Windows;
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
            ShowDashboard();
        }

        private void ShowDashboard()
        {
            dashboardPage = new DashboardPage();
            dashboardPage.TotalOrdersClicked += DashboardPage_TotalOrdersClicked;
            dashboardPage.PickListClicked += DashboardPage_PickListClicked;
            // Wire up other tile events here
            DashboardContent.Content = dashboardPage;
        }

        private void OrderDashboardMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void SalesDashboardMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new SalesDashboardPage();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }

        private void RefundManagementMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new RefundManagementPage();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }

        private void InventoryManagementMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new InventoryManagementPage();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }

        // Existing tile click handlers...
        private void DashboardPage_TotalOrdersClicked(object sender, RoutedEventArgs e)
        {
            var page = new TotalOrdersPage();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        private void DashboardPage_PickListClicked(object sender, RoutedEventArgs e)
        {
            var page = new PickListPage();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        // ...other tile handlers
    }
}