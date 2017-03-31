using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Add_Element_Request
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string UnitTypeCode { get; set; }

        public string Price { get; set; }

        public int ShelfId { get; set; }

        public double WarningQuantity { get; set; }
        public string Remark { get; set; }
    }

    public class Edit_Element_Request : Add_Element_Request
    {
        public int Id { get; set; }
    }
}
