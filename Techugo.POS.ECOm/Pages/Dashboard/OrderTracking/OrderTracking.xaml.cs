using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Config;

namespace Techugo.POS.ECOm.Pages.Dashboard.OrderTracking
{
    /// <summary>
    /// Interaction logic for OrderTracking.xaml
    /// </summary>
    public partial class OrderTracking : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<TrackingItem> _orderData;
        private Window _orderDetailsPopUpWindow;
        public ObservableCollection<TrackingItem> orderData
        {
            get => _orderData;
            set
            {
                _orderData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(orderData)));
            }
        }

        private string _totalOrdersText;
        public string TotalOrdersText
        {
            get => _totalOrdersText;
            set
            {
                _totalOrdersText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalOrdersText)));
            }
        }
        private int _inProgress;
        public int InProgress
        {
            get => _inProgress;
            set => SetField(ref _inProgress, value);
        }

        private int _completed;
        public int Completed
        {
            get => _completed;
            set => SetField(ref _completed, value);
        }
        public OrderTracking()
        {
            InitializeComponent();
            DataContext = this;
            orderData = new ObservableCollection<TrackingItem>();
            _apiService = ApiServiceFactory.Create();
            LoadOrdersData();
        }
        private async void LoadOrdersData()
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
            TrackingResponse orderResponse = await _apiService.GetAsync<TrackingResponse>("order/tracking-list?page=1&limit=10&date=" + formattedDate + "");
            if (orderResponse != null)
            {

                orderData.Clear();
                foreach (var or in orderResponse.Data)
                {
                    TrackingItem trackingItem = new TrackingItem
                    {
                        OrderID = or.OrderID,
                        OrderNo = or.OrderNo,
                        Status = GetStatusToShow(or).Status,
                        Message = or.Rider.Name != null ? or.Rider.Name + " " + GetStatusToShow(or).Description : GetStatusToShow(or).Description,
                        Rider = or.Rider
                    };
                    orderData.Add(trackingItem);
                }

                Completed = orderData.Count(item => item.Status == "Completed");
                InProgress = orderData.Count(item => item.Status != "Completed");

            }
        }

        private (string Status, string Description) GetStatusToShow(TrackingItem trackingItem)
        {
            if (trackingItem == null) return (null, null);

            // Resolve preference: RiderStatus > BranchStatus > Status
            var resolvedStatus = trackingItem.RiderStatus ?? trackingItem.BranchStatus ?? trackingItem.Status;
            if (string.IsNullOrWhiteSpace(resolvedStatus))
                return (null, null);

            // First try direct lookup by key (case-insensitive)
            if (StatusConstants.Mappings.TryGetValue(resolvedStatus, out var mapped))
            {
                return mapped;
            }

            // Then try matching by the display Status value inside the mapping
            var matchByValue = StatusConstants.Mappings.Values
                .FirstOrDefault(v => string.Equals(v.Status, resolvedStatus, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(matchByValue.Status) || !string.IsNullOrEmpty(matchByValue.Description))
                return matchByValue;

            // Try a relaxed normalization (ignore spaces/hyphens/case)
            string Normalize(string s) => (s ?? string.Empty).Replace(" ", "").Replace("-", "").ToLowerInvariant();
            var normResolved = Normalize(resolvedStatus);
            foreach (var kvp in StatusConstants.Mappings)
            {
                if (Normalize(kvp.Key) == normResolved) return kvp.Value;
                if (Normalize(kvp.Value.Status) == normResolved) return kvp.Value;
            }

            // Fallback: return the resolved status with no description
            return (resolvedStatus, null);
        }

        private class StatusMappingItem
        {
            public string Status { get; set; }
            public string Description { get; set; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
