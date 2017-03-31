using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class PickListView
    {
        public int Id { get; set; }

        public string ElementName { get; set; }

        public string BomName { get; set; }

        public string PartName { get; set; }

        public double Quantity { get; set; }

        public double StockOutQuantity { get; set; }

        public string IsFeedback { get; set; }

        public string Purpose { get; set; }

        public int ParentId { get; set; }

        public string Addtime { get; set; }

        public string AddUserName { get; set; }

        public string StockOutTime { get; set; }

        public string StockOutUserName { get; set; }
    }

    public class PickDetailView
    {

        [DisplayName("原材料名称")]
        public string ElementName { get; set; }
        [DisplayName("所属Bom")]
        public string BomName { get; set; }
        [DisplayName("所属部件")]
        public string PartName { get; set; }
        [DisplayName("申请数量")]
        public double Quantity { get; set; }

        [DisplayName("数量单位")]
        public string UnitName { get; set; }
        [DisplayName("是否已发料")]
        public string IsFeedback { get; set; }
        [DisplayName("发料数量")]
        public double StockOutQuantity { get; set; }

        [DisplayName("库存数量")]
        public string StockQuantity { get; set; }

        [DisplayName("申请目的")]
        public string Purpose { get; set; }

        [DisplayName("申请时间")]
        public string Addtime { get; set; }
        [DisplayName("申请人")]
        public string AddUserName { get; set; }
        [DisplayName("发料时间")]
        public string StockOutTime { get; set; }
        [DisplayName("发料人")]
        public string StockOutUserName { get; set; }
    }

    public class Pick_ForAdd_View
    {
        public int ElementId { get; set; }
        public string ElementName { get; set; }

        public int BomId { get; set; }
        public string BomName { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }


        public double Quantity { get; set; }
        public string UnitName { get; set; }
        public int IsSelect { get; set; }

        public string StockQuantity { get; set; }

    }

    public class PickOut_ForAdd_View
    {
        public int IsSelect { get; set; }
        public string ElementName { get; set; }

        public string BomName { get; set; }
        public string PartName { get; set; }
        public double Quantity { get; set; }

        public double ALStockOutQuantity { get; set; }
        public double StockOutQuantity { get; set; }
        public string UnitName { get; set; }
        public string StockQuantity { get; set; }
        public string ShelfName { get; set; }

        public int BomId { get; set; }
        public int ElementId { get; set; }
        public int Id { get; set; }

        public string AddUserName { get; set; }

        public string Addtime { get; set; }

        public int ShelfId { get; set; }
    }


    public class PickOutListView
    {
        public int Id { get; set; }
        public string Addtime { get; set; }
        public string AddUserName { get; set; }
        public string Purpose { get; set; }
        public string IsFeedback { get; set; }
        public string StockOutTime { get; set; }
        public string StockOutUserName { get; set; }
    }


    public class Export_PickOut_Request
    {
        public string ColumnTitles { get; set; }

        public string Columns { get; set; }

        public string FileName { get; set; }

        public string ExpData { get; set; }
    }

}
