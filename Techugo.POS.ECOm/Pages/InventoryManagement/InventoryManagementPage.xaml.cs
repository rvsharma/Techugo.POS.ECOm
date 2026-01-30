using MaterialDesignColors;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;
using Techugo.POS.ECOm.Services;

namespace Techugo.POS.ECOm.Pages
{
    public partial class InventoryManagementPage : UserControl, INotifyPropertyChanged
    {
        private DispatcherTimer _barcodeTimer;
        private StringBuilder _buffer = new StringBuilder();
        private DateTime _lastKeystroke = DateTime.MinValue;

        private DispatcherTimer _searchTimer;

        public event RoutedEventHandler BackRequested;
        private readonly ApiService _apiService;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<InventoryVM> _inventoryData;
        public ObservableCollection<InventoryVM> inventoryData
        {
            get => _inventoryData;
            set
            {
                _inventoryData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(inventoryData)));
            }
        }
        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set => SetField(ref _totalItems, value);
        }

        private int _activeProducts;
        public int ActiveProducts
        {
            get => _activeProducts;
            set => SetField(ref _activeProducts, value);
        }

        private int _inactiveProducts;
        public int InactiveProducts
        {
            get => _inactiveProducts;
            set => SetField(ref _inactiveProducts, value);
        }

        private int _outOfStock;
        public int OutOfStock
        {
            get => _outOfStock;
            set => SetField(ref _outOfStock, value);
        }
        private string _totalItemText;
        public string TotalItemText
        {
            get => _totalItemText;
            set
            {
                _totalItemText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalItemText)));
            }
        }

        private ObservableCollection<Brand> _brandList;
        public ObservableCollection<Brand> BrandList
        {
            get => _brandList;
            set
            {
                _brandList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrandList)));
            }
        }
        public int? SelectedBrandID { get; set; }

        // Prevent Toggle Checked/Unchecked handlers from reacting to programmatic bindings/initialization
        private bool _suppressToggleEvents = true;
        private string _pendingSearchText;

        public InventoryManagementPage()
        {
            InitializeComponent();
            DataContext = this;
            inventoryData = new ObservableCollection<InventoryVM>();
            BrandList = new ObservableCollection<Brand>();
            _apiService = ApiServiceFactory.Create();
            var queryData = new { page = 1, limit = 1000, brandId = (int?)null, categoryIds = System.Array.Empty<object>(), search = (string)null, division = SelectedBrandID };
            // start loading; suppression remains true until LoadInventoryData finishes
            LoadInventoryData(queryData);
            GetBrands();
            SelectedBrandID = null;

            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _searchTimer.Tick += SearchTimer_Tick;

            //_barcodeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            //_barcodeTimer.Tick += BarcodeTimer_Tick;

            //// Capture scanner input globally
            //this.PreviewTextInput += OnPreviewTextInput;

        }

        //private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    // Reset buffer if too slow (user typing)
        //    if ((DateTime.Now - _lastKeystroke).TotalMilliseconds > 1000)
        //        _buffer.Clear();

        //    _lastKeystroke = DateTime.Now;
        //    _buffer.Append(e.Text);

        //    // Restart flush timer
        //    _barcodeTimer.Stop();
        //    _barcodeTimer.Start();
        //}

        //private void BarcodeTimer_Tick(object sender, EventArgs e)
        //{
        //    _barcodeTimer.Stop();

        //    string scanned = _buffer.ToString();
        //    _buffer.Clear();

        //    if (!string.IsNullOrEmpty(scanned))
        //    {
        //        OnBarcodeScanned(scanned);

        //        // Call API with scanned code
        //        var queryData = new
        //        {

        //            search = scanned,

        //        };

        //        //UpdateInventory(queryData);
        //    }
        //}
        //public event EventHandler<string> BarcodeScanned;
        //private void OnBarcodeScanned(string code)
        //{
        //    BarcodeScanned?.Invoke(this, code);
        //}

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

        private async void LoadInventoryData(object queryData)
        {
            try
            {

                ItemListResponse itemList = await _apiService.PostAsync<ItemListResponse>("item/item-list", queryData);
                if (itemList != null && itemList.Data != null)
                {
                    inventoryData.Clear();

                    foreach (var or in itemList.Data)
                    {
                        InventoryVM inventory = new InventoryVM();

                        inventory.ItemID = or.ID;
                        inventory.ImagePath = or.ItemImages.Count > 0 ? or.ItemImages[0].ImagePath : string.Empty;
                        inventory.ItemName = or.ItemName;

                        inventory.Barcode = or.Barcode;
                        inventory.Brand = or.BrandDetails?.BrandName;
                        inventory.Category = or.CategoryDetails?.Category;
                        inventory.SPrice = or.SPrice;
                        inventory.MRP = or.MRP;
                        inventory.IsActive = or.ItemBranchLists != null && or.ItemBranchLists.Count > 0
                            ? or.ItemBranchLists[0].IsActive
                            : "Inactive";

                        inventoryData.Add(inventory);
                    }
                    UpdateStats();
                }
                TotalItemText = $"Products Inventory ({itemList?.TotalItems})";
            }
            finally
            {
                // Allow handlers to run only after initial population finishes
                _suppressToggleEvents = false;
            }
        }

        private async void GetBrands()
        {
            try
            {
                BrandResponse riderListResponse = await _apiService.GetAsync<BrandResponse>("common/brand-list");
                var list = riderListResponse?.Data ?? new List<Brand>();
                list.Insert(0, new Brand { BrandID = null, BrandName = "All Brands" });
                BrandList = new ObservableCollection<Brand>(list);
            }
            catch
            {

            }


        }
        private void UpdateStats()
        {
            // compute totals from inventoryData
            TotalItems = inventoryData?.Count ?? 0;

            // ActiveProducts: check IsActive string; adjust if you use bool
            ActiveProducts = inventoryData?.Count(i => string.Equals(i.IsActive, "Active", System.StringComparison.OrdinalIgnoreCase)) ?? 0;

            OutOfStock = TotalItems - ActiveProducts;

            // OutOfStock: uses StockQuantity; ensure StockQuantity is populated from API mapping
            // OutOfStock = inventoryData?.Count(i => i.StockQuantity <= 0) ?? 0;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }

        private void SwitchToggle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ToggleButton tb)
            {
                // mark control as user-initiated
                tb.Tag = "user";
            }
        }

        private void SwitchToggle_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is ToggleButton tb && (e.Key == Key.Space || e.Key == Key.Enter))
            {
                // mark control as user-initiated for keyboard toggles
                tb.Tag = "user";
            }
        }

        private async void SwitchToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_suppressToggleEvents) return;
            if (sender is ToggleButton tb)
            {
                // Only proceed if user interaction set the flag
                if (!(tb.Tag is string tag && tag == "user")) return;
                tb.Tag = null; // clear flag
            }
            await HandleToggleChangedAsync(sender, true);
        }

        private async void SwitchToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_suppressToggleEvents) return;
            if (sender is ToggleButton tb)
            {
                if (!(tb.Tag is string tag && tag == "user")) return;
                tb.Tag = null;
            }
            await HandleToggleChangedAsync(sender, false);
        }

        // Centralized handler: optimistic UI update, call API, revert on failure
        private async Task HandleToggleChangedAsync(object sender, bool isChecked)
        {
            if (sender is not ToggleButton toggle) return;
            if (toggle.DataContext is not InventoryVM vm) return;

            // If still suppressing (extra safety)
            if (_suppressToggleEvents) return;

            string previousValue = vm.IsActive;
            string newValue = isChecked ? "Active" : "Inactive";

            // Optimistically update UI
            vm.IsActive = newValue;

            // Suppress events while we programmatically update or when awaiting API
            _suppressToggleEvents = true;
            try
            {
                var payload = new { status = newValue };

                // Example endpoint — replace with the actual API contract
                BaseResponse result = await _apiService.PutAsync<BaseResponse>("item/update-item-stock/" + vm.ItemID, payload);
                if (result != null && result.Success == true)
                {
                    SnackbarService.Enqueue("Item updated successfully");
                    var queryData = new { page = 1, limit = 1000, brandId = (int?)null, categoryIds = System.Array.Empty<object>(), search = (string)null, division = SelectedBrandID };

                    LoadInventoryData(queryData);
                }
                else
                {
                    // revert on server-side failure
                    vm.IsActive = previousValue;
                    SnackbarService.Enqueue("Failed to update item.");
                }
            }
            catch (System.Exception)
            {
                // revert UI on failure and inform user
                vm.IsActive = previousValue;
                SnackbarService.Enqueue("Failed to update item.");
            }
            finally
            {
                // Re-enable handlers
                _suppressToggleEvents = false;
            }
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

        public class BrandResponse : BaseResponse
        {
            [JsonPropertyName("data")]
            public List<Brand> Data { get; set; } = new List<Brand>();
        }
        public class Brand
        {
            public int? BrandID { get; set; }
            public string BrandName { get; set; }
            public int? Division { get; set; }


        }

        private void BrandFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandFilterCombo.SelectedValue is int brandId)
            {
                SelectedBrandID = brandId;
            }
            var queryData = new { page = 1, limit = 1000, brandId = (int?)null, categoryIds = System.Array.Empty<object>(), search = (string)null, division = SelectedBrandID };
            LoadInventoryData(queryData);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Store latest text
            _pendingSearchText = string.IsNullOrWhiteSpace(textBox.Text) ? null : textBox.Text;

            // Reset debounce timer
            _searchTimer.Stop();
            _searchTimer.Start();
        }
        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();

            // Call your API function here
            var queryData = new { page = 1, limit = 1000, brandId = (int?)null, categoryIds = System.Array.Empty<object>(), search = _pendingSearchText, division = SelectedBrandID };

            LoadInventoryData(queryData);
        }
    }
}