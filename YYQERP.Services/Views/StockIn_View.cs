using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class StockInView
    {
        public int ElementId { get; set; }
        public int ShelfId { get; set; }
        public double Quantity { get; set; }
        public string UnitTypeCode { get; set; }
      
    }

    public class StockIn_ForAdd_ByProductView
    {

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ShelfId { get; set; }


        public double Quantity { get; set; }
        public string UnitName { get; set; }

        public string Remark { get; set; }

        public int IsSelect { get; set; }

        public int ItemType { get; set; }

        public string ItemTypeText { get; set; }

        public string UnitTypeCode { get; set; }

    }

    public class StockInListView
    {
        public int Id { get; set; }

        public string ShelfName { get; set; }

        public string ItemName { get; set; }

        public string ItemTypeText { get; set; }
        public double Quantity { get; set; }

        public string StockQuantity { get; set; }

        public string UnitName { get; set; }

        public string Addtime { get; set; }

        public string GroupGuid { get; set; }

     

     

        public int ShowWarn { get; set; }

        public string Reason { get; set; }

    }

 
}
