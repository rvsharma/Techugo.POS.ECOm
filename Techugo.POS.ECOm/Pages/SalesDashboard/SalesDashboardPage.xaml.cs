using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages
{
    public partial class SalesDashboardPage : UserControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler BackRequested;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ApiService _apiService;
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (value > DateTime.Today)
                    _selectedDate = DateTime.Today; // clamp to today
                else
                    _selectedDate = value;
                _ = LoadDataAsync();
                // Trigger analytics refresh logic here
            }
        }


        private  WeeklyChartViewModel _viewModel;

        public SalesDashboardPage()
        {
            InitializeComponent();
            _apiService = ApiServiceFactory.Create();
            _viewModel = new WeeklyChartViewModel { MaxY = 1000, Days = new List<string>() };
            DataContext = _viewModel;
            _ = LoadDataAsync();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }

        public async Task LoadDataAsync()
        {
            string formattedDate = SelectedDate.ToString("yyyy-MM-dd");
            try
            {
                SalesAnalyticsResponse response = await _apiService.GetAsync<SalesAnalyticsResponse>("branch/sales-analytics?date=" + formattedDate + "");
                if (response != null && response.Success && response.Data != null)
                {
                    var data = response.Data;
                    AverageDailySalesTextBlock.Text = $"{data.AverageDailySales:N0}";
                    AverageDailySalesIncreaseTextBlock.Text = data.AverageDailySalesIncrease + " increase from yesterday";
                    TodayEarningsTextBlock.Text = $"{data.TodayEarnings:N0}";
                    TodayEarningsIncreaseTextBlock.Text = data.TodayEarningsIncrease + " increase from yesterday";
                    // Update chart with actual weekly comparison data
                    var overlayValues = new ChartValues<double>(
                        data.WeeklyComparison.Select(wc => (double)wc.Amount));
                    var maxY = CeilingToNearest(overlayValues.Max(), 200);
                    var baseRemainderValues = new ChartValues<double>(
                        overlayValues.Select(v => maxY - v));
                   
                    _viewModel.Days = data.WeeklyComparison.Select(wc => wc.Day).ToList();
                    _viewModel.MaxY = maxY == 0 ? 1000 : maxY;
                    _viewModel.SeriesCollection = new SeriesCollection
                    {
                        new StackedColumnSeries
                        {
                            Title = string.Empty,
                            Values = overlayValues,
                            Fill = new SolidColorBrush(Color.FromRgb(30, 136, 229)),
                            MaxColumnWidth = 40,
                            DataLabels = false,
                            LabelPoint = point => $"?{point.Y:N0}",
                            IsHitTestVisible = false
                        },
                        new StackedColumnSeries
                        {
                            Title = string.Empty,
                            Values = baseRemainderValues,
                            Fill = new SolidColorBrush(Color.FromArgb(80, 144, 202, 249)),
                            MaxColumnWidth = 40,
                            DataLabels = false,
                            IsHitTestVisible = false
                        }
                    };
                    //DataContext = _viewModel;

                }
            }
            catch
            {
                // Handle exceptions (e.g., log error, show message to user)
            }
        }

        private double CeilingToNearest(double value, int step)
        {
            return ((int)System.Math.Ceiling(value / step)) * step;
        }

        public class WeeklyChartViewModel : INotifyPropertyChanged
        {
            private SeriesCollection _seriesCollection;
            public SeriesCollection SeriesCollection
            {
                get => _seriesCollection;
                set { _seriesCollection = value; OnPropertyChanged(nameof(SeriesCollection)); }
            }

            private List<string> _days;
            public List<string> Days
            {
                get => _days;
                set { _days = value; OnPropertyChanged(nameof(Days)); }
            }

            private double _maxY;
            public double MaxY
            {
                get => _maxY;
                set { _maxY = value; OnPropertyChanged(nameof(MaxY)); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            if (picker.SelectedDate.HasValue && picker.SelectedDate.Value > DateTime.Today)
            {
                SelectedDate = DateTime.Today; // reset to today
            }
            else
            {
                SelectedDate = picker.SelectedDate.Value;
            }
            

        }
    }

}