using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model.ViewModel
{
    public class InventoryVM
    {
        public int ItemBranchID { get; set; }
        public int ItemID { get; set; }
        public string ImagePath { get; set; }
        public string ItemName { get; set; }
        public bool IsVeg { get; set; }
        public string Barcode { get; set; }
        public string Brand { get; set; }
        public string Category  { get; set; }
        public decimal SPrice { get; set; }
        public decimal MRP { get; set; }
        public int StockQuantity { get; set; }
        public string IsActive { get; set; }

    }
}
