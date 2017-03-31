using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class StockListView
    {
        public int Id { get; set; }

        public string ShelfName { get; set; }

        public string ElementName { get; set; }



        public string ItemTypeText { get; set; }


        public double Quantity { get; set; }

        public double WarnQuantity { get; set; }

        public string UnitType { get; set; }

        public string FirstInTime { get; set; }

        public string LastInTime { get; set; }

        public string LastOutTime { get; set; }
    }

    public class StockDetailView
    {
        [DisplayName("库位")]
        public string ShelfName { get; set; }
        [DisplayName("物品名称")]
        public string ElementName { get; set; }
        [DisplayName("物品属性")]
        public string ItemTypeText { get; set; }
        [DisplayName("数量")]
        public string Quantity { get; set; }
        [DisplayName("单位")]
        public string UnitType { get; set; }
        [DisplayName("首次入库时间")]
        public string FirstInTime { get; set; }
        [DisplayName("最近入库时间")]
        public string LastInTime { get; set; }
        [DisplayName("最近入库数量")]
        public string LastInQuantity { get; set; }
        [DisplayName("最近出库时间")]
        public string LastOutTime { get; set; }
        [DisplayName("最近出库数量")]
        public string LastOutQuantity { get; set; }
        [DisplayName("创建时间")]
        public string Addtime { get; set; }
        [DisplayName("创建用户")]
        public string AddUserName { get; set; }
        [DisplayName("修改时间")]
        public string Modifytime { get; set; }
        [DisplayName("修改用户")]
        public string ModifyUserName { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }
    }

    public class StockWarnListView
    {
        public int Id { get; set; }

        public string ShelfName { get; set; }

        public string ElementName { get; set; }



        public string ItemTypeText { get; set; }

        public double Quantity { get; set; }

        public string UnitType { get; set; }

        public double WarnQuantity { get; set; }

        public double LastOutQuantity { get; set; }

        public string LastOutTime { get; set; }

        public double PickQuantity { get; set; }
    }
}
