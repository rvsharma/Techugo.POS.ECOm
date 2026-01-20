using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages.Notification
{
    /// <summary>
    /// Interaction logic for QuickListWindow.xaml
    /// </summary>
    public partial class QuickListWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ApiService _apiService;

        private ObservableCollection<NotificationItem> _notifications;
        public ObservableCollection<NotificationItem> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notifications)));
            }
        }
        public QuickListWindow()
        {
            InitializeComponent();
            _apiService = ApiServiceFactory.Create();
            Deactivated += QuickListWindow_Deactivated;
            DataContext = this;
            Notifications = new ObservableCollection<NotificationItem>();
            _ = GetNotificationsAsync();
        }

        private void QuickListWindow_Deactivated(object? sender, EventArgs e)
        {
            // When the popup loses activation we want to close it.
            // However closing immediately can cause undesirable z-order behavior
            // (the whole app may move behind other applications). To avoid
            // stealing focus when the user clicked into another application we:
            //  - delay briefly so the system updates activation,
            //  - check if any window in our app is active (meaning the user
            //    clicked somewhere inside this app), and only then ensure the
            //    owner is activated after closing the popup.
            Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    // Determine whether any window in our application has activation.
                    bool appWindowActive = Application.Current.Windows.OfType<Window>().Any(w => w.IsActive);

                    // Close the popup in all cases.
                    Close();

                    // If activation moved to a window in our app, ensure the owner is brought to front.
                    if (appWindowActive && Owner is Window owner && !owner.IsActive)
                    {
                        try { owner.Activate(); } catch { /* ignore activation failures */ }
                    }
                }
                catch
                {
                    // swallow exceptions to avoid crashing on deactivation.
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        public async Task GetNotificationsAsync()
        {
            try
            {
                NotificationsResponse data = await _apiService.GetAsync<NotificationsResponse>("branch/notification-list?page=1&limit=100");
                if (data != null)
                {
                    Notifications = new ObservableCollection<NotificationItem>(
                        data.Data.Where(x => x.IsRead == false).Select(n => new NotificationItem
                        {
                            Title = n.Title,
                            Message = n.Message,
                            CreatedAt = n.CreatedAt
                        })
                    );
                }
            }
            catch (Exception ex) { /* consider logging ex */ }
        }

        public class NotificationItem
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        //public string SelectedItem => ItemsListBox.SelectedItem as string;

    }
}
