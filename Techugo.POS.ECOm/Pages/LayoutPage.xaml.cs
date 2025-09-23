using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECOm.Pages.Dashboard;

namespace Techugo.POS.ECOm.Pages
{
    public partial class LayoutPage : UserControl
    {
        public LayoutPage()
        {
            InitializeComponent();
            ShowDashboard();
        }

        private void ShowDashboard()
        {
            var dashboardPage = new DashboardPage();
            dashboardPage.TotalOrdersClicked += DashboardPage_TotalOrdersClicked;
            dashboardPage.PickListClicked += DashboardPage_PickListClicked;
            dashboardPage.AssignRiderClicked += DashboardPage_AssignRiderClicked;
            dashboardPage.PendingDeliveryClicked += DashboardPage_PendingDeliveryClicked;
            dashboardPage.DeliveredClicked += DashboardPage_DeliveredClicked;
            dashboardPage.RejectedClicked += DashboardPage_RejectedClicked;
            dashboardPage.PartialReturnsClicked += DashboardPage_PartialReturnsClicked;
            dashboardPage.CarryForwardClicked += DashboardPage_CarryForwardClicked;

            SetPageContent(dashboardPage);
        }

        // Menu navigation handlers should call SetPageContent(newPage)
        private void OrderDashboardMenu_Click(object sender, RoutedEventArgs e)
        {
            SetPageContent(new DashboardPage());
        }
        private void SalesDashboardMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new SalesDashboardPage();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void RefundManagementMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new RefundManagementPage();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void InventoryManagementMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new InventoryManagementPage();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_TotalOrdersClicked(object sender, RoutedEventArgs e)
        {
            var page = new TotalOrdersPage();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_PickListClicked(object sender, RoutedEventArgs e)
        {
            var page = new PickListPage();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_AssignRiderClicked(object sender, RoutedEventArgs e)
        {
            var page = new AssignRider();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_PendingDeliveryClicked(object sender, RoutedEventArgs e)
        {
            var page = new PendingDelivery();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_DeliveredClicked(object sender, RoutedEventArgs e)
        {
            var page = new Delivered();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_RejectedClicked(object sender, RoutedEventArgs e)
        {
            var page = new Rejected();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_PartialReturnsClicked(object sender, RoutedEventArgs e)
        {
            var page = new PartialReturns();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_CarryForwardClicked(object sender, RoutedEventArgs e)
        {
            var page = new CarryForward();
            page.BackRequested += (s, args) => SetPageContent(new DashboardPage());
            SetPageContent(page);
        }

        public void SetPageContent(UserControl page)
        {
            PageContent.Content = page;
        }
    }
}
