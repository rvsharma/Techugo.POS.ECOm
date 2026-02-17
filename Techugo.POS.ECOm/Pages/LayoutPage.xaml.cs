using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Pages.Dashboard;
using Techugo.POS.ECOm.Pages.Dashboard.OrderTracking;
using Techugo.POS.ECOm.Pages.Notification;
using Techugo.POS.ECOm.Services;
using static Techugo.POS.ECOm.Pages.Notification.QuickListWindow;

namespace Techugo.POS.ECOm.Pages
{
    public partial class LayoutPage : UserControl, INotifyPropertyChanged
    {

        private DispatcherTimer _barcodeTimer;
        private StringBuilder _buffer = new StringBuilder();
        private DateTime _lastKeystroke = DateTime.MinValue;

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ApiService _apiService;
        private string _notificationCount;
        public string UnreadAlertsCount
        {
            get => _notificationCount;
            set
            {
                _notificationCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnreadAlertsCount)));
            }
        }
        private Window _newOrderPopupWindow; // Add this field

        // DependencyProperty so XAML bindings (DataTrigger) can observe the state
        public static readonly DependencyProperty IsMenuExpandedProperty =
            DependencyProperty.Register(nameof(IsMenuExpanded), typeof(bool), typeof(LayoutPage), new PropertyMetadata(true));

        public bool IsMenuExpanded
        {
            get => (bool)GetValue(IsMenuExpandedProperty);
            set => SetValue(IsMenuExpandedProperty, value);
        }

        public LayoutPage()
        {
            InitializeComponent();
            _apiService = ApiServiceFactory.Create();
            _ = LoadNotificationCountAsync();
            DataContext = this;
            ShowDashboard();
            UnreadAlertsCount = "Last updated: " + DateTime.Now.ToString("h:mm:ss tt");
            // ensure initial width matches expanded state
            LeftMenu.Width = IsMenuExpanded ? 240 : 48;
            RefreshButton.Click += RefreshButton_Click;


            _barcodeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _barcodeTimer.Tick += BarcodeTimer_Tick;

            // Capture scanner input globally
            this.PreviewTextInput += OnPreviewTextInput;
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Reset buffer if too slow (user typing)
            if ((DateTime.Now - _lastKeystroke).TotalMilliseconds > 1000)
                _buffer.Clear();

            _lastKeystroke = DateTime.Now;
            _buffer.Append(e.Text);

            // Restart flush timer
            _barcodeTimer.Stop();
            _barcodeTimer.Start();
        }

        private void BarcodeTimer_Tick(object sender, EventArgs e)
        {
            _barcodeTimer.Stop();

            string scanned = _buffer.ToString();
            _buffer.Clear();

            if (!string.IsNullOrEmpty(scanned))
            {
                OnBarcodeScanned(scanned);

                // Call API with scanned code
                var queryData = new
                {

                    search = scanned,

                };

                UpdateInventory(queryData);
            }
        }
        public event EventHandler<string> BarcodeScanned;
        private void OnBarcodeScanned(string code)
        {
            BarcodeScanned?.Invoke(this, code);
        }

        private async void UpdateInventory(object queryData)
        {
            // Works for "ITEM-3\r" too
            string pattern = @"(?<=-)\d+(?=\r|$)";

            string result = Regex.Match(queryData.ToString(), pattern).Value;

            if (string.IsNullOrEmpty(result))
                return;
            int n;
            bool isNumeric = int.TryParse(result, out n);

            if (isNumeric)
            {
                var data = new
                {
                    item_id = n,
                    quantity = 1
                };

                BaseResponse response = await _apiService.PostAsyncUnAuth<BaseResponse>("buy/add", data);
                if (response != null)
                {
                    SnackbarService.Enqueue(response.Message);
                }
            }


        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {


            var popup = new QuickListWindow()
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.Manual,
                ShowInTaskbar = false
            };

            // position popup immediately below the RefreshButton (notification icon)
            // get screen coordinates for the bottom-left of the button
            var button = RefreshButton;
            var screenPoint = button.PointToScreen(new Point(0, button.ActualHeight));

            // convert device pixels to WPF units
            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget != null)
            {
                var transform = source.CompositionTarget.TransformFromDevice;
                var target = transform.Transform(screenPoint);

                // small horizontal offset to align popup to right edge of button if needed
                popup.Left = target.X;
                popup.Top = target.Y + 24;

                // ensure popup stays within primary screen bounds (basic clamp)
                var primary = SystemParameters.WorkArea;
                if (popup.Left + popup.Width > primary.Right) popup.Left = primary.Right - popup.Width - 8;
                if (popup.Top + popup.Height > primary.Bottom) popup.Top = primary.Bottom - popup.Height - 8;
                if (popup.Left < primary.Left) popup.Left = primary.Left + 8;
                if (popup.Top < primary.Top) popup.Top = primary.Top + 8;
            }
            else
            {
                // fallback: center over main window
                popup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            // show non-modal (no overlay). window will close automatically on deactivated.
            popup.Show();
            _ = LoadNotificationCountAsync();
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
            dashboardPage.OrderTrackingClicked += DashboardPage_OrderTrackingClicked;
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
        private void DashboardPage_OrderTrackingClicked(object sender, RoutedEventArgs e)
        {
            var page = new OrderTracking();
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

        // Toggle expand/collapse on button click and start storyboard
        private void ExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            IsMenuExpanded = !IsMenuExpanded;

            var sbKey = IsMenuExpanded ? "ExpandMenu" : "CollapseMenu";
            if (TryFindResource(sbKey) is Storyboard sb)
            {
                // Ensure the storyboard targets the named LeftMenu
                sb.Begin(this, true);
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

        public async Task LoadNotificationCountAsync()
        {
            try
            {

                NotificationsResponse data = await _apiService.GetAsync<NotificationsResponse>("branch/notification-list?page=1&limit=100");
                if (data != null)
                {

                    int unread = data.Data?.Count(n => n != null && !n.IsRead) ?? 0;
                    UnreadAlertsCount = unread.ToString();
                    UnreadAlertsCountName.Margin = unread > 9 ? new Thickness(0, 0, 3, 0) : new Thickness(0, 0, 6, 0);
                }

            }
            catch (Exception ex) { /* consider logging ex */ }
            // TODO: Parse and display data in your dashboard UI
        }
    }
}
