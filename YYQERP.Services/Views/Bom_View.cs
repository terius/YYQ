using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class BomDetailEditView
    {
        public int Id { get; set; }
        //  public int BomId { get; set; }

        public int ElementId { get; set; }
        public string ElementName { get; set; }

        public string PartCode { get; set; }
        public string PartName { get; set; }
        public double Quantity { get; set; }
        public string UnitName { get; set; }
        public string Remark { get; set; }

        public string Addtime { get; set; }

        public string ShelfName { get; set; }

        public int IsSelect { get; set; }

    }

    public class BomListInPartView
    {
        public int Id { get; set; }
        public string BomName { get; set; }
        public string Remark { get; set; }
    }

    public class BomListView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ModelName { get; set; }
        public int ModelId { get; set; }

        public string Remark { get; set; }

        public string Addtime { get; set; }
    }


    public class Export_Bom_View
    {
        public string BomName { get; set; }
        public string ElementName { get; set; }

        public string PartName { get; set; }
        public double Quantity { get; set; }
        public string UnitName { get; set; }

        public string ShelfName { get; set; }
        public string Remark { get; set; }

        public string Addtime { get; set; }





    }
}
