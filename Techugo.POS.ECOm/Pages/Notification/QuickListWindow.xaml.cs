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
            // close when focus moves away to mimic a lightweight popover
            Close();
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
