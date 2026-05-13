using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Services;

namespace Techugo.POS.ECOm.Pages
{
    public partial class RefundManagementPage : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ApiService _apiService;
        private DispatcherTimer _searchTimer;

        private ObservableCollection<RefundVM> _refundData;
        public ObservableCollection<RefundVM> RefundData
        {
            get => _refundData;
            set
            {
                _refundData = value;
                OnPropertyChanged(nameof(RefundData));
            }
        }

        private int _totalRefunds;
        public int TotalRefunds
        {
            get => _totalRefunds;
            set => SetField(ref _totalRefunds, value);
        }

        private int _initiatedCount;
        public int InitiatedCount
        {
            get => _initiatedCount;
            set => SetField(ref _initiatedCount, value);
        }

        private int _processingCount;
        public int ProcessingCount
        {
            get => _processingCount;
            set => SetField(ref _processingCount, value);
        }

        private int _completedCount;
        public int CompletedCount
        {
            get => _completedCount;
            set => SetField(ref _completedCount, value);
        }

        private string _selectedPaymentMode = "COD";
        public string SelectedPaymentMode
        {
            get => _selectedPaymentMode;
            set
            {
                if (SetField(ref _selectedPaymentMode, value))
                {
                    CurrentPage = 1;
                    LoadRefundData();
                }
            }
        }

        private string _refundOrderText;
        public string RefundOrderText
        {
            get => _refundOrderText;
            set => SetField(ref _refundOrderText, value);
        }

        private const int ITEMS_PER_PAGE = 10;
        private int _currentPage = 1;
        private int _totalPages = 1;

        public int CurrentPage
        {
            get => _currentPage;
            set => SetField(ref _currentPage, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetField(ref _totalPages, value);
        }

        private string _paginationText;
        public string PaginationText
        {
            get => _paginationText;
            set => SetField(ref _paginationText, value);
        }

        private bool _canGoToPreviousPage;
        public bool CanGoToPreviousPage
        {
            get => _canGoToPreviousPage;
            set => SetField(ref _canGoToPreviousPage, value);
        }

        private bool _canGoToNextPage;
        public bool CanGoToNextPage
        {
            get => _canGoToNextPage;
            set => SetField(ref _canGoToNextPage, value);
        }

        private string _pendingSearchText;

        public RefundManagementPage()
        {
            InitializeComponent();
            DataContext = this;
            RefundData = new ObservableCollection<RefundVM>();
            _apiService = ApiServiceFactory.Create();

            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _searchTimer.Tick += SearchTimer_Tick;

            LoadRefundData();
        }

        private async void LoadRefundData()
        {
            try
            {
                string endpoint = $"order/refund-management?page={CurrentPage}&limit={ITEMS_PER_PAGE}&paymentMode={SelectedPaymentMode}";
                
                if (!string.IsNullOrWhiteSpace(_pendingSearchText))
                {
                    endpoint += $"&search={_pendingSearchText}";
                }

                RefundManagementResponse response = await _apiService.GetAsync<RefundManagementResponse>(endpoint);
                TotalPages = 0;
                TotalRefunds = 0;
                RefundData.Clear();
                if (response != null && response.Success && response.Data != null)
                {
                    
                    foreach (var refund in response.Data)
                    {
                        if(refund?.OrderMaster?.OrderDetails != null)

                        {
                            string address = string.Empty;
                            if (refund.OrderMaster?.OrderAddress != null)
                            {
                                var parts = new List<string>();
                                if (!string.IsNullOrWhiteSpace(refund.OrderMaster.OrderAddress.HouseNo))
                                    parts.Add(refund.OrderMaster.OrderAddress.HouseNo);
                                if (!string.IsNullOrWhiteSpace(refund.OrderMaster.OrderAddress.StreetNo))
                                    parts.Add(refund.OrderMaster.OrderAddress.StreetNo);
                                if (!string.IsNullOrWhiteSpace(refund.OrderMaster.OrderAddress.State))
                                    parts.Add(refund.OrderMaster.OrderAddress.State);
                                if (!string.IsNullOrWhiteSpace(refund.OrderMaster.OrderAddress.City))
                                    parts.Add(refund.OrderMaster.OrderAddress.City);
                                if (!string.IsNullOrWhiteSpace(refund.OrderMaster.OrderAddress.Pincode))
                                    parts.Add(refund.OrderMaster.OrderAddress.Pincode);

                                address = string.Join(", ", parts);
                            }


                            try
                            {
                                var createdAt = refund.CreatedAt;
                                if (createdAt.Kind == DateTimeKind.Utc)
                                    refund.CreatedAt = createdAt.ToLocalTime();
                                else if (createdAt.Kind == DateTimeKind.Unspecified)
                                    refund.CreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc).ToLocalTime();
                                else
                                    refund.CreatedAt = createdAt;
                            }
                            catch
                            {
                                // Fallback: assign raw value if any unexpected issue occurs
                                refund.CreatedAt = refund.CreatedAt;
                            }

                            RefundVM refundVm = new RefundVM
                            {
                                //OrderID = refund.OrderMaster?.OrderID ?? string.Empty,
                                OrderNo = refund.OrderMaster?.OrderNo ?? string.Empty,
                                Amount = refund.Amount,


                                CreatedAt = refund.CreatedAt,
                                ShortAddress = address.Length > 20 ? address.Substring(0, 20) + "..." : address,
                                Status = GetRefundStatus(refund.OrderMaster?.OrderID) ?? string.Empty,
                                CustomerName = refund.OrderMaster?.Customer?.CustomerName == null ? refund.OrderMaster?.OrderAddress?.Name : refund.OrderMaster?.Customer?.CustomerName,
                                MobileNo = refund.OrderMaster?.OrderAddress?.MobileNo,
                                OrderDetails = refund.OrderMaster?.OrderDetails != null ? System.Text.Json.JsonSerializer.Deserialize<List<OrderDetail>>(refund.OrderMaster.OrderDetails.ToString()) : null
                            };
                            RefundData.Add(refundVm);
                        }

                        
                    }

                    TotalPages = response.TotalPages;
                    TotalRefunds = response.TotalItems;
                    UpdateStats();
                    UpdatePaginationUI();
                    UpdateRefundOrderText();
                }
                else
                {
                    RefundData.Clear();
                    UpdateStats();
                    UpdatePaginationUI();
                    UpdateRefundOrderText();
                    PaginationSection.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Enqueue("Failed to load refund data");
            }
        }

        private string GetRefundStatus(string orderId)
        {
            // This is a placeholder - you might want to get actual status from the API
            // For now, we'll randomize or base it on some logic
            var random = new Random();
            int statusIndex = random.Next(3);
            return statusIndex switch
            {
                0 => "Initiated",
                1 => "Processing",
                _ => "Completed"
            };
        }

        private void UpdateStats()
        {
            InitiatedCount = RefundData.Count(r => r.Status == "Initiated");
            ProcessingCount = RefundData.Count(r => r.Status == "Processing");
            CompletedCount = RefundData.Count(r => r.Status == "Completed");
        }

        private void UpdatePaginationUI()
        {
            CanGoToPreviousPage = CurrentPage > 1;
            CanGoToNextPage = CurrentPage < TotalPages;
            PaginationText = $"Page {CurrentPage} of {TotalPages}";
        }

        private void UpdateRefundOrderText()
        {
            RefundOrderText = $"Refund Orders ({TotalRefunds})";
        }

        private void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadRefundData();
            }
        }

        private void GoToNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadRefundData();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            _pendingSearchText = string.IsNullOrWhiteSpace(textBox.Text) ? null : textBox.Text;
            CurrentPage = 1;

            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            LoadRefundData();
        }

        private void PaymentModeFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentModeFilterCombo.SelectedItem is ComboBoxItem item && item.Tag is string paymentMode)
            {
                SelectedPaymentMode = paymentMode;
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            GoToNextPage();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public class RefundVM
        {
            public string OrderID { get; set; }
            public string OrderNo { get; set; }
            public decimal Amount { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; }
            public string ShortAddress { get; set; }
            public string CustomerName { get; set; }
            public string MobileNo { get; set; }
            public List<OrderDetail> OrderDetails { get; set; }
        }
    }
}