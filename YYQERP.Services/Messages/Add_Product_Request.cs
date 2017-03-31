using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Add_Product_Request
    {
        public string Aliases { get; set; }

        public int ModelId { get; set; }

     

        public string Price { get; set; }

        public string UnitTypeCode { get; set; }

        public int ItemType { get; set; }


        public string CreateDate { get; set; }

        public string Remark { get; set; }

    }

    public class Edit_Product_Request : Add_Product_Request
    {
        public int Id { get; set; }
    }
}
