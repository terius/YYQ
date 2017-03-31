using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class StockOut_ForAdd_View
    {
        public int ElementId { get; set; }
        public string ElementName { get; set; }

        public int BomId { get; set; }
        public string BomName { get; set; }
        public int ShelfId { get; set; }

        public string ShelfName { get; set; }
        public double Quantity { get; set; }
        public string UnitName { get; set; }

        public string Remark { get; set; }

        public int IsSelect { get; set; }

        public string PartName { get; set; }

        public string StockQuantity { get; set; }

        public int ItemType { get; set; }

        public int? PickId { get; set; }

    }

    public class StockOut_ForAdd_ByProductView
    {

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ShelfId { get; set; }

        public string ShelfName { get; set; }
        public double Quantity { get; set; }
        public string UnitName { get; set; }

        public string Remark { get; set; }

        public int IsSelect { get; set; }



        public string StockQuantity { get; set; }

        public int ItemType { get; set; }

    }

    //public class StockOutListView
    //{
    //    public int Id { get; set; }
    //    public string Addtime { get; set; }
    //    public string Reason { get; set; }
    //    public IList<StockOutListDetailView> Details { get; set; }
    //}

    public class StockOutListDetailView
    {

        public int Id { get; set; }

      

        public string ShelfName { get; set; }

        public string ElementName { get; set; }

        public string ItemTypeText { get; set; }
        public double Quantity { get; set; }

        public string StockQuantity { get; set; }

        public string UnitName { get; set; }

        public string Addtime { get; set; }

      

        public string BomName { get; set; }

        
        
       

        public int ShowWarn { get; set; }

        public string Reason { get; set; }

        public string GroupGuid { get; set; }
    }


    public class StockOutDetailDeleteView
    {
        public bool IsEle { get; set; }
        public double Quantity { get; set; }

        public int? ElementId { get; set; }

        public int? ProductId { get; set; }

        public int ShelfId { get; set; }
    }
}
