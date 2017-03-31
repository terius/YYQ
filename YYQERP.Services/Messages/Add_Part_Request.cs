using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Add_Part_Request
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Remark { get; set; }

    }

    public class Edit_Part_Request : Add_Part_Request
    {
        public int Id { get; set; }
    }
}
