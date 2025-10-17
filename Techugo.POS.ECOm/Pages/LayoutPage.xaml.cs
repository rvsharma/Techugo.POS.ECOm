using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECOm.Pages.Dashboard;

namespace Techugo.POS.ECOm.Pages
{
    public partial class LayoutPage : UserControl
    {
        private Window _newOrderPopupWindow; // Add this field

        public LayoutPage()
        {
            InitializeComponent();
            ShowDashboard();
        }

        private DashboardPage CreateDashboardPage()
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
            return dashboardPage;
        }

        private void ShowDashboard()
        {
            SetPageContent(CreateDashboardPage());
        }

        // Menu navigation handlers should call SetPageContent(newPage)
        private void OrderDashboardMenu_Click(object sender, RoutedEventArgs e)
        {
            SetPageContent(CreateDashboardPage());
        }
        private void SalesDashboardMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new SalesDashboardPage();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void RefundManagementMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new RefundManagementPage();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void InventoryManagementMenu_Click(object sender, RoutedEventArgs e)
        {
            var page = new InventoryManagementPage();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_TotalOrdersClicked(object sender, RoutedEventArgs e)
        {
            var page = new TotalOrdersPage();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_PickListClicked(object sender, RoutedEventArgs e)
        {
            var page = new PickListPage();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_AssignRiderClicked(object sender, RoutedEventArgs e)
        {
            var page = new AssignRider();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_PendingDeliveryClicked(object sender, RoutedEventArgs e)
        {
            var page = new PendingDelivery();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_DeliveredClicked(object sender, RoutedEventArgs e)
        {
            var page = new Delivered();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_RejectedClicked(object sender, RoutedEventArgs e)
        {
            var page = new Rejected();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_PartialReturnsClicked(object sender, RoutedEventArgs e)
        {
            var page = new PartialReturns();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void DashboardPage_CarryForwardClicked(object sender, RoutedEventArgs e)
        {
            var page = new CarryForward();
            page.BackRequested += (s, args) => SetPageContent(CreateDashboardPage());
            SetPageContent(page);
        }
        private void NewOrderAlerts_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Play sound
            try
            {
                var soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds", "neworder.wav");
                if (System.IO.File.Exists(soundPath))
                {
                    SoundPlayer player = new SoundPlayer(soundPath);
                    player.Play();
                }
            }
            catch { /* Handle exceptions if needed */ }

            var popup = new NewOrderPopUp();
            popup.AcceptOrderClicked += CloseNewOrderPopUp;
            popup.RejectOrderClicked += CloseNewOrderPopUp;

            // Option 1: Show as overlay in PageContent (replace current content)
            // SetPageContent(popup);

            // Option 2: Show as a dialog/modal (recommended for popups)
            // If you want a true modal, consider using a Window or a custom overlay.
            // Example:
            _newOrderPopupWindow = new Window
            {
                Content = popup,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Owner = Application.Current.MainWindow,
                Width = 400,
                Height = 220,
                ShowInTaskbar = false
            };
            _newOrderPopupWindow.ShowDialog();

        }
        public void SetPageContent(UserControl page)
        {
            PageContent.Content = page;
        }
        private void CloseNewOrderPopUp(object sender, RoutedEventArgs e)
        {
            if (_newOrderPopupWindow != null)
            {
                _newOrderPopupWindow.Close();
                _newOrderPopupWindow = null;
            }
        }
        public void ShowPickListPage()
        {
            var assignRider = new AssignRider();
            // wire BackRequested same as Dashboard menu handling
            assignRider.BackRequested += (s, args) =>
            {
                var dashboard = CreateDashboardPage();
                SetPageContent(dashboard);
            };
            SetPageContent(assignRider);
        }
    }
}
