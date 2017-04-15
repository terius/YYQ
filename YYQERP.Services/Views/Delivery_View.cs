using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class DeliveryListView
    {
        public int Id { get; set; }

        public string Remark { get; set; }
        public string Addtime { get; set; }
        public string Customer { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string TotalAmount { get; set; }

        public string Sender { get; set; }
        public string Manager { get; set; }
        public string AddUserName { get; set; }

        public IList<DeliveryDetailView> Details { get; set; }
    }

    public class DeliveryDetailView
    {

        public string Type { get; set; }
        public string Model { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }

        public string Price { get; set; }

        public string TotalPrice { get; set; }

        public string Remark { get; set; }

    }

    public class Delivery_Add_View
    {
        public string Remark { get; set; }
        public string Customer { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string TotalAmount { get; set; }

        public string Sender { get; set; }
        public string Manager { get; set; }

        public IList<DeliveryDetail_ForAdd_View> Details { get; set; }
    }


    public class DeliveryDetail_ForAdd_View
    {
        public Nullable<int> ElementId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public double Quantity { get; set; }

        public string Remark { get; set; }
        public string UnitTypeCode { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }

        public string Type { get; set; }
        public string Model { get; set; }

        public string Unit { get; set; }

    }
}
