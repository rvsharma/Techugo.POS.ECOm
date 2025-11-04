
using System.Text.Json.Serialization;

namespace Techugo.POS.ECom.Model
{
    public class StockListResponse: BaseResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public List<StockItemBranch> Data { get; set; } = new();
    }

    public class StockItemBranch
    {
        [JsonPropertyName("ItemBranchID")]
        public int ItemBranchID { get; set; }

        [JsonPropertyName("IsActive")]
        public string IsActive { get; set; }

        [JsonPropertyName("AdminStatus")]
        public string AdminStatus { get; set; }

        [JsonPropertyName("ItemID")]
        public int ItemID { get; set; }

        [JsonPropertyName("BranchID")]
        public string BranchID { get; set; }

        [JsonPropertyName("Item")]
        public StockItem Item { get; set; }
    }

    public class StockItem
    {
        [JsonPropertyName("ItemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("IsVeg")]
        public bool IsVeg { get; set; }

        [JsonPropertyName("Barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("HindiName")]
        public string HindiName { get; set; }

        [JsonPropertyName("Size")]
        public string Size { get; set; }

        [JsonPropertyName("UOM")]
        public string UOM { get; set; }

        [JsonPropertyName("SPrice")]
        public decimal SPrice { get; set; }

        [JsonPropertyName("MRP")]
        public decimal MRP { get; set; }

        [JsonPropertyName("CanSubscribe")]
        public bool CanSubscribe { get; set; }

        [JsonPropertyName("Status")]
        public string Status { get; set; }

        [JsonPropertyName("IsActive")]
        public string IsActive { get; set; }

        [JsonPropertyName("BrandID")]
        public int BrandID { get; set; }

        [JsonPropertyName("CategoryID")]
        public int CategoryID { get; set; }

        [JsonPropertyName("CategoryDetails")]
        public CategoryDetails CategoryDetails { get; set; }

        [JsonPropertyName("BrandDetails")]
        public BrandDetails BrandDetails { get; set; }

        [JsonPropertyName("ItemImages")]
        public List<StockItemImage> ItemImages { get; set; } = new();
    }

    public class CategoryDetails
    {
        [JsonPropertyName("Category")]
        public string Category { get; set; }
    }

    public class BrandDetails
    {
        [JsonPropertyName("BrandName")]
        public string BrandName { get; set; }
    }

    public class StockItemImage
    {
        [JsonPropertyName("ItemImgID")]
        public int ItemImgID { get; set; }

        [JsonPropertyName("ImagePath")]
        public string ImagePath { get; set; }
    }
}