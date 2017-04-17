using System.Collections.Generic;

namespace YYQERP.Infrastructure
{
    public class DeliveryForPrint
    {
        public string Customer { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }

        public string SerialNo { get; set; }

        public string TotalAmountUp { get; set; }

        public string TotalAmount { get; set; }
        public string Sender { get; set; }
        public string Manager { get; set; }

        public IList<DeliveryForPrint_Detail> Details { get; set; }
    }
    public class DeliveryForPrint_Detail
    {
      
        public string Model { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }

        public string Price { get; set; }

        public string TotalPrice { get; set; }

        public string Remark { get; set; }
    }

}
