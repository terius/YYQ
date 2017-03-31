using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Add_Shelf_Request
    {

        public string Code { get; set; }

        public string Remark { get; set; }

    }

    public class Edit_Shelf_Request : Add_Shelf_Request
    {
        public int Id { get; set; }
    }
}
