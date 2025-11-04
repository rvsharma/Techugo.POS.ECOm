using MaterialDesignColors;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECom.Model.ViewModel;
using Techugo.POS.ECOm.ApiClient;

namespace Techugo.POS.ECOm.Pages
{
    public partial class InventoryManagementPage : UserControl
    {
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

        public InventoryManagementPage()
        {
            InitializeComponent();
            DataContext = this;
            inventoryData = new ObservableCollection<InventoryVM>();
            _apiService = ApiServiceFactory.Create();
            LoadInventoryData();
        }

        private async void LoadInventoryData()
        {
            var data = new { page = 1, limit = 10, brandId = (int?)null, categoryIds = new[] { 1, 2, 3 }, search = (string)null, status = "InStock" };
            StockListResponse stocklist = await _apiService.PostAsync<StockListResponse>("item/stock-list", data);
            if (stocklist != null && stocklist.Data != null)
            {

                inventoryData.Clear();

                foreach (var or in stocklist.Data)
                {

                    InventoryVM inventory = new InventoryVM();
                    inventory.ItemBranchID = or.ItemBranchID;
                    inventory.ItemID = or.ItemID;
                    inventory.ImagePath = or.Item.ItemImages[0].ImagePath;
                    inventory.ItemName = or.Item.ItemName;
                    inventory.IsVeg = or.Item.IsVeg;
                    inventory.Barcode = or.Item.Barcode;
                    inventory.Brand = or.Item.BrandDetails.BrandName;
                    inventory.Category = or.Item.CategoryDetails.Category;
                    inventory.SPrice = or.Item.SPrice;
                    inventory.MRP = or.Item.MRP;
                    //inventory.StockQuantity = or.Item.StockQuantity;
                    inventory.IsActive = or.IsActive;
                    inventoryData.Add(inventory);

                }

            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, new RoutedEventArgs());
        }
    }
}