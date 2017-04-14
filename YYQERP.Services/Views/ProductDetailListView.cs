using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class ProductDetailListView
    {

        public int ElementId { get; set; }
        public string ElementName { get; set; }

        public int BomId { get; set; }
     //   public string BomName { get; set; }

        public int HalfProductId { get; set; }
      //  public string HalfProductName { get; set; }

        public double Quantity { get; set; }

        public string UnitName { get; set; }

        public string ItemName { get; set; }

        public int IsSelect { get; set; }
    }


    public class ProductView
    {
        public int Id { get; set; }
        public string Model { get; set; }

        public string Name { get; set; }
    }
}
