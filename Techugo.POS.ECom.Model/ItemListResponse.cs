using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class ItemListResponse: BaseResponse
    {
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public List<Item> Data { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        [JsonPropertyName("ItemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("Size")]
        public string Size { get; set; }

        [JsonPropertyName("UOM")]
        public string UOM { get; set; }

        [JsonPropertyName("SPrice")]
        public decimal SPrice { get; set; }

        [JsonPropertyName("MRP")]
        public decimal MRP { get; set; }

        [JsonPropertyName("Barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("CategoryDetails")]
        public CategoryDetails CategoryDetails { get; set; }

        [JsonPropertyName("BrandDetails")]
        public BrandDetails BrandDetails { get; set; }

        [JsonPropertyName("ItemImages")]
        public List<ItemImage> ItemImages { get; set; }

        [JsonPropertyName("ItemBranchLists")]
        public List<ItemBranchList> ItemBranchLists { get; set; }
    }

    public class ItemBranchList
    {
        [JsonPropertyName("IsActive")]
        public string IsActive { get; set; }
    }
}