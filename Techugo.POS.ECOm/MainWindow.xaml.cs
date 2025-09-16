using System.Windows;
using Techugo.POS.ECOm.Pages;
using Techugo.POS.ECOm.Pages.Dashboard;

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
            dashboardPage.AssignRiderClicked += DashboardPage_AssignRiderClicked;
            dashboardPage.PendingDeliveryClicked += DashboardPage_PendingDeliveryClicked;
            dashboardPage.DeliveredClicked += DashboardPage_DeliveredClicked;
            dashboardPage.RejectedClicked += DashboardPage_RejectedClicked;
            dashboardPage.PartialReturnsClicked += DashboardPage_PartialReturnsClicked;
            dashboardPage.CarryForwardClicked += DashboardPage_CarryForwardClicked;

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

        private void DashboardPage_AssignRiderClicked(object sender, RoutedEventArgs e)
        {
            var page = new AssignRider();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        private void DashboardPage_PendingDeliveryClicked(object sender, RoutedEventArgs e)
        {
            var page = new PendingDelivery();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        private void DashboardPage_DeliveredClicked(object sender, RoutedEventArgs e)
        {
            var page = new Delivered();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        private void DashboardPage_RejectedClicked(object sender, RoutedEventArgs e)
        {
            var page = new Rejected();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        private void DashboardPage_PartialReturnsClicked(object sender, RoutedEventArgs e)
        {
            var page = new PartialReturns();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        private void DashboardPage_CarryForwardClicked(object sender, RoutedEventArgs e)
        {
            var page = new CarryForward();
            page.BackRequested += (s, args) => ShowDashboard();
            DashboardContent.Content = page;
        }
        // ...other tile handlers
    }
}